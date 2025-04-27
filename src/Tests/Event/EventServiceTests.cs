using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Application.Exceptions;
using EventsWebApp.Application.Service;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Moq;
using Models = EventsWebApp.Domain.Models;

namespace Tests.Event;

public class EventServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _mockRepository = new Mock<IRepositoryManager>();
        _mockMapper = new Mock<IMapper>();
        _mockFileService = new Mock<IFileService>();
        _mockEmailService = new Mock<IEmailService>();

        _eventService = new EventService(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockFileService.Object,
            _mockEmailService.Object);
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsEventDto_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventEntity = new Models.Event
        {
            Id = eventId,
            Name = "Test Event",
            Description = "This is a test event",
            StartDateTime = DateTime.UtcNow,
            Location = "Test Location",
            Category = EventCategory.Concert,
            MaxParticipants = 100
        };

        var eventDto = new EventDto
        {
            Id = eventId,
            Name = "Test Event",
            Description = "This is a test event",
            StartDateTime = DateTime.UtcNow,
            Location = "Test Location",
            Category = EventCategory.Concert,
            MaxParticipants = 100
        };

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockMapper.Setup(mapper => mapper.Map<EventDto>(eventEntity))
            .Returns(eventDto);

        // Act
        var result = await _eventService.GetEventByIdAsync(eventId, false, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventDto.Id, result.Id);
        Assert.Equal(eventDto.Name, result.Name);
        Assert.Equal(eventDto.Description, result.Description);
    }

    [Fact]
    public async Task GetEventByIdAsync_ThrowsException_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Event)null!);

        // Act & Assert
        await Assert.ThrowsAsync<EventByIdNotFoundException>(() =>
            _eventService.GetEventByIdAsync(eventId, false, CancellationToken.None));
    }

    [Fact]
    public async Task CreateEventAsync_ReturnsCreatedEventDto()
    {
        // Arrange
        var eventForCreationDto = new EventForManipulationDto
        {
            Name = "New Event",
            Description = "This is a new event",
            StartDateTime = DateTime.UtcNow,
            Location = "New Location",
            Category = EventCategory.Party,
            MaxParticipants = 50
        };

        var eventEntity = new Models.Event
        {
            Id = Guid.NewGuid(),
            Name = "New Event",
            Description = "This is a new event",
            StartDateTime = DateTime.UtcNow,
            Location = "New Location",
            Category = EventCategory.Party,
            MaxParticipants = 50
        };

        var eventDto = new EventDto
        {
            Id = eventEntity.Id,
            Name = "New Event",
            Description = "This is a new event",
            StartDateTime = DateTime.UtcNow,
            Location = "New Location",
            Category = EventCategory.Party,
            MaxParticipants = 50
        };

        _mockMapper.Setup(mapper => mapper.Map<Models.Event>(eventForCreationDto))
            .Returns(eventEntity);

        _mockMapper.Setup(mapper => mapper.Map<EventDto>(eventEntity))
            .Returns(eventDto);

        _mockRepository.Setup(repo => repo.Event.CreateEvent(eventEntity))
            .Verifiable();

        _mockRepository.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _eventService.CreateEventAsync(eventForCreationDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventDto.Id, result.Id);
        Assert.Equal(eventDto.Name, result.Name);
        Assert.Equal(eventDto.Description, result.Description);
    }

    [Fact]
    public async Task DeleteEventAsync_DeletesEventAndImage()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var imagePath = "image_to_delete.jpg";

        var eventEntity = new Models.Event
        {
            Id = eventId,
            ImagePath = imagePath
        };

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockFileService.Setup(fs => fs.DeleteFile(imagePath))
            .Verifiable();

        _mockRepository.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventService.DeleteEventAsync(eventId, false, CancellationToken.None);

        // Assert
        _mockFileService.Verify(fs => fs.DeleteFile(imagePath), Times.Once);
    }

    [Fact]
    public async Task UploadImageAsync_UpdatesImagePath_AndDeletesOldImage()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var oldFilePath = "old_image.jpg";
        var newFileName = "new_image.jpg";

        var eventEntity = new Models.Event
        {
            Id = eventId,
            ImagePath = oldFilePath
        };

        var imageFileMock = new Mock<IFormFile>();
        imageFileMock.Setup(f => f.Length).Returns(500 * 1024);

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockFileService.Setup(fs => fs.SaveFileAsync(imageFileMock.Object, It.IsAny<string[]>()))
            .ReturnsAsync(newFileName);

        _mockFileService.Setup(fs => fs.DeleteFile(oldFilePath))
            .Verifiable();

        _mockRepository.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventService.UploadImageAsync(eventId, false, imageFileMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(newFileName, eventEntity.ImagePath);
        _mockFileService.Verify(fs => fs.DeleteFile(oldFilePath), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_SendsNotification_WhenStartDateTimeOrLocationChanged()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventEntity = new Models.Event
        {
            Id = eventId,
            Name = "Old Event",
            Description = "Old description",
            StartDateTime = DateTime.UtcNow,
            Location = "Old Location",
            Category = EventCategory.Concert,
            MaxParticipants = 100
        };

        var updateDto = new EventForManipulationDto
        {
            Name = "Updated Event",
            Description = "Updated description",
            StartDateTime = DateTime.UtcNow.AddDays(1),
            Location = "New Location",
            Category = EventCategory.Party,
            MaxParticipants = 50
        };

        var users = new List<User>
        {
            new User { Email = "user1@example.com" },
            new User { Email = "user2@example.com" }
        };

        var pagedUsers = new PagedList<User>(users, 1, 1, 2);
        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockMapper.Setup(mapper => mapper.Map(updateDto, eventEntity))
            .Callback(() =>
            {
                eventEntity.Name = updateDto.Name;
                eventEntity.Description = updateDto.Description;
                eventEntity.StartDateTime = (DateTime)updateDto.StartDateTime;
                eventEntity.Location = updateDto.Location;
                eventEntity.Category = updateDto.Category;
                eventEntity.MaxParticipants = updateDto.MaxParticipants;
            });

        _mockRepository.Setup(repo => repo.User.GetParticipantUsersByEventAsync(
            eventId, It.IsAny<UserParameters>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedUsers);

        _mockRepository.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventService.UpdateEventAsync(eventId, updateDto, false, CancellationToken.None);

        // Assert
        Assert.Equal(updateDto.StartDateTime, eventEntity.StartDateTime);
        Assert.Equal(updateDto.Location, eventEntity.Location);

        _mockEmailService.Verify(emailService => emailService.SendEmailToManyAsync(
            It.Is<IEnumerable<string>>(emails => emails.Contains("user1@example.com") && emails.Contains("user2@example.com")),
            "Event Details Updated",
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_DeletesImageAndClearsImagePath()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var imagePath = "image_to_delete.jpg";

        var eventEntity = new Models.Event
        {
            Id = eventId,
            ImagePath = imagePath
        };

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockFileService.Setup(fs => fs.DeleteFile(imagePath))
            .Verifiable();

        _mockRepository.Setup(repo => repo.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventService.DeleteImageAsync(eventId, false, CancellationToken.None);

        // Assert
        Assert.Empty(eventEntity.ImagePath);
        _mockFileService.Verify(fs => fs.DeleteFile(imagePath), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_ThrowsException_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Event)null!);

        // Act & Assert
        await Assert.ThrowsAsync<EventByIdNotFoundException>(() =>
            _eventService.DeleteImageAsync(eventId, false, CancellationToken.None));
    }

    [Fact]
    public async Task GetImageAsync_ReturnsImageDetails_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var imagePath = "test_image.png";
        var fileBytes = new byte[] { 0x01, 0x02, 0x03 };
        var contentType = "image/png";

        var eventEntity = new Models.Event
        {
            Id = eventId,
            ImagePath = imagePath
        };

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockFileService.Setup(fs => fs.GetFileAsync(imagePath))
            .ReturnsAsync((fileBytes, contentType, imagePath));

        // Act
        var result = await _eventService.GetImageAsync(eventId, false, CancellationToken.None);

        // Assert
        Assert.Equal(fileBytes, result.fileBytes);
        Assert.Equal(contentType, result.contentType);
        Assert.Equal(imagePath, result.filename);
    }

    [Fact]
    public async Task GetImageAsync_ThrowsException_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _mockRepository.Setup(repo => repo.Event.GetEventByIdAsync(eventId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Event)null!);

        // Act & Assert
        await Assert.ThrowsAsync<EventByIdNotFoundException>(() =>
            _eventService.GetImageAsync(eventId, false, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllEventAsync_ReturnsEventsWithMetaData()
    {
        // Arrange
        var eventParameters = new EventParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var eventEntities = new List<Models.Event>
        {
            new Models.Event
            {
                Id = Guid.NewGuid(),
                Name = "Event 1",
                Description = "Description 1",
                StartDateTime = DateTime.UtcNow,
                Location = "Location 1",
                Category = EventCategory.Concert,
                MaxParticipants = 100
            },
            new Models.Event
            {
                Id = Guid.NewGuid(),
                Name = "Event 2",
                Description = "Description 2",
                StartDateTime = DateTime.UtcNow.AddDays(1),
                Location = "Location 2",
                Category = EventCategory.Party,
                MaxParticipants = 50
            }
        };

        var eventDtos = eventEntities.Select(e => new EventDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            StartDateTime = e.StartDateTime,
            Location = e.Location,
            Category = e.Category,
            MaxParticipants = e.MaxParticipants
        }).ToList();

        var metaData = new MetaData
        {
            CurrentPage = 1,
            TotalPages = 1,
            PageSize = 10,
            TotalCount = eventEntities.Count
        };

        var pagedEvents = new PagedList<Models.Event>(eventEntities, eventEntities.Count, eventParameters.PageNumber, eventParameters.PageSize);

        _mockRepository.Setup(repo => repo.Event.GetAllEventsAsync(eventParameters, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedEvents);

        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<EventDto>>(eventEntities))
            .Returns(eventDtos);

        // Act
        var (events, returnedMetaData) = await _eventService.GetAllEventAsync(eventParameters, false, CancellationToken.None);

        // Assert
        Assert.NotNull(events);
        Assert.Equal(eventDtos.Count, events.Count());
        Assert.Equal(metaData.CurrentPage, returnedMetaData.CurrentPage);
        Assert.Equal(metaData.TotalPages, returnedMetaData.TotalPages);
        Assert.Equal(metaData.PageSize, returnedMetaData.PageSize);
        Assert.Equal(metaData.TotalCount, returnedMetaData.TotalCount);

        var firstEvent = events.First();
        var firstEventEntity = eventEntities.First();
        Assert.Equal(firstEventEntity.Id, firstEvent.Id);
        Assert.Equal(firstEventEntity.Name, firstEvent.Name);
    }

    [Fact]
    public async Task GetEventByNameAsync_ReturnsEventDto_WhenEventExists()
    {
        // Arrange
        var eventName = "Test Event";
        var eventEntity = new Models.Event
        {
            Id = Guid.NewGuid(),
            Name = eventName,
            Description = "This is a test event",
            StartDateTime = DateTime.UtcNow,
            Location = "Test Location",
            Category = EventCategory.Concert,
            MaxParticipants = 100
        };

        var eventDto = new EventDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            StartDateTime = eventEntity.StartDateTime,
            Location = eventEntity.Location,
            Category = eventEntity.Category,
            MaxParticipants = eventEntity.MaxParticipants
        };

        _mockRepository.Setup(repo => repo.Event.GetEventByNameAsync(eventName, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventEntity);

        _mockMapper.Setup(mapper => mapper.Map<EventDto>(eventEntity))
            .Returns(eventDto);

        // Act
        var result = await _eventService.GetEventByNameAsync(eventName, false, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventDto.Id, result.Id);
        Assert.Equal(eventDto.Name, result.Name);
        Assert.Equal(eventDto.Description, result.Description);
    }

    [Fact]
    public async Task GetEventByNameAsync_ThrowsException_WhenEventDoesNotExist()
    {
        // Arrange
        var eventName = "NonExistent Event";

        _mockRepository.Setup(repo => repo.Event.GetEventByNameAsync(eventName, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Event)null!);

        // Act & Assert
        await Assert.ThrowsAsync<EventByNameNotFoundException>(() =>
            _eventService.GetEventByNameAsync(eventName, false, CancellationToken.None));
    }
}

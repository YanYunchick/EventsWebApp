using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Models;
using EventsWebApp.Infrastructure.Repository;
using EventsWebApp.Infrastructure.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Event;

public class EventRepositoryTests
{
    private readonly RepositoryContext _context;
    private readonly IEventRepository _eventRepository;

    public EventRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<RepositoryContext>()
            .UseInMemoryDatabase(databaseName: "TestEventsWebAppDB")
            .Options;

        _context = new RepositoryContext(options);
        _eventRepository = new EventRepository(_context);
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsEvent_WhenEventExists()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var testEvent = new EventsWebApp.Domain.Models.Event
        {
            Id = eventId,
            Name = "Test Event",
            Description = "Test description",
            StartDateTime = DateTime.UtcNow,
            Location = "Test Location",
            Category = EventCategory.Concert,
            MaxParticipants = 100
        };

        _context.Events!.Add(testEvent);
        await _context.SaveChangesAsync();

        // Act
        var result = await _eventRepository.GetEventByIdAsync(eventId, trackChanges: false, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testEvent.Id, result.Id);
        Assert.Equal(testEvent.Name, result.Name);
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsNull_WhenEventDoesNotExist()
    {
        // Arrange
        var nonExistentEventId = Guid.NewGuid();

        // Act
        var result = await _eventRepository.GetEventByIdAsync(nonExistentEventId, trackChanges: false, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEvent_ShouldCreateNewEvent()
    {
        // Arrange
        var newEvent = new EventsWebApp.Domain.Models.Event
        {
            Id = Guid.NewGuid(),
            Name = "New Test Event",
            Description = "New Test Description",
            StartDateTime = DateTime.UtcNow,
            Location = "New Test Location",
            Category = EventCategory.Party,
            MaxParticipants = 50
        };

        // Act
        _eventRepository.CreateEvent(newEvent);
        await _context.SaveChangesAsync();

        // Assert
        var addedEvent = await _context.Events!.FirstOrDefaultAsync(e => e.Id == newEvent.Id);
        Assert.NotNull(addedEvent);
        Assert.Equal(newEvent.Id, addedEvent.Id);
        Assert.Equal(newEvent.Name, addedEvent.Name);
        Assert.Equal(newEvent.Description, addedEvent.Description);
        Assert.Equal(newEvent.StartDateTime, addedEvent.StartDateTime);
        Assert.Equal(newEvent.Location, addedEvent.Location);
        Assert.Equal(newEvent.Category, addedEvent.Category);
        Assert.Equal(newEvent.MaxParticipants, addedEvent.MaxParticipants);
    }
}

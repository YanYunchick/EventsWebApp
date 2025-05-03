using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Application.Exceptions;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;

[assembly: InternalsVisibleTo("Tests")]
namespace EventsWebApp.Application.Service;

internal sealed class EventService : IEventService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _memoryCache;

    public EventService(
        IRepositoryManager repository, 
        IMapper mapper, 
        IFileService fileService,
        IEmailService emailService,
        IMemoryCache memoryCache)
    {
        _repository = repository;
        _mapper = mapper;
        _fileService = fileService;
        _emailService = emailService;
        _memoryCache = memoryCache;
    }

    public async Task<(IEnumerable<EventDto> events, MetaData metaData)> GetAllEventAsync(
        EventParameters eventParameters, bool trackChanges, CancellationToken cancellationToken)
    {
        var eventsWithMetaData = await _repository.Event
            .GetAllEventsAsync(eventParameters, trackChanges, cancellationToken);

        var eventsDto = _mapper.Map<IEnumerable<EventDto>>(eventsWithMetaData);
        return (
            events: eventsDto,
            metaData: eventsWithMetaData.MetaData);
    }

    public async Task<EventDto> GetEventByIdAsync(Guid eventId, bool trackChanges, CancellationToken cancellationToken)
    {
        var eventEntity = await _repository.Event.GetEventByIdAsync(eventId, trackChanges, cancellationToken);
        if (eventEntity is null)
            throw new EventByIdNotFoundException(eventId);

        var eventDto = _mapper.Map<EventDto>(eventEntity);
        return eventDto;
    }

    public async Task<EventDto> GetEventByNameAsync(string name, bool trackChanges, CancellationToken cancellationToken)
    {
        var eventEntity = await _repository.Event.GetEventByNameAsync(name, trackChanges, cancellationToken);
        if (eventEntity is null)
            throw new EventByNameNotFoundException(name);

        var eventDto = _mapper.Map<EventDto>(eventEntity);
        return eventDto;
    }

    public async Task<EventDto> CreateEventAsync(EventForManipulationDto eventDto, CancellationToken cancellationToken)
    {
        var eventEntity = _mapper.Map<Event>(eventDto);

        _repository.Event.CreateEvent(eventEntity);
        await _repository.SaveAsync(cancellationToken);

        var eventToReturn = _mapper.Map<EventDto>(eventEntity);
        return eventToReturn;
    }

    public async Task DeleteEventAsync(Guid eventId, bool trackChanges, CancellationToken cancellationToken)
    {
        var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);

        _repository.Event.DeleteEvent(eventEntity);
        await _repository.SaveAsync(cancellationToken);

        _fileService.DeleteFile(eventEntity.ImagePath!);
    }

    public async Task UpdateEventAsync(
        Guid eventId, 
        EventForManipulationDto eventForUpdateDto,
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);
        bool startDateTimeOrLocationChanged =
            !eventEntity.StartDateTime.Equals(eventForUpdateDto.StartDateTime) ||
            !eventEntity.Location.Equals(eventForUpdateDto.Location);

        _mapper.Map(eventForUpdateDto, eventEntity);

        await _repository.SaveAsync(cancellationToken);

        if (startDateTimeOrLocationChanged)
        {
            await SendNotificationAboutUpdateEventAsync(eventEntity, cancellationToken);
        }
    }

    private async Task SendNotificationAboutUpdateEventAsync(Event eventEntity, CancellationToken cancellationToken)
    {
        var users = await _repository.ParticipantUser.GetParticipantUsersByEventAsync(
                eventEntity.Id,
                new UserParameters { },
                trackChanges: false,
                cancellationToken);

        if (users.Any())
        {
            string emailBody = GenerateEventUpdateEmailBody(eventEntity);

            var userEmails = users.Select(u => u.Email).Where(email => !string.IsNullOrEmpty(email));
            if (userEmails.Count() != 0)
            {
                await _emailService.SendEmailToManyAsync(
                userEmails!,
                subject: "Event Details Updated",
                emailBody);
            }
        }
    }

    private string GenerateEventUpdateEmailBody(Event eventEntity)
    {
        return $@"
        <html>
        <body>
            <h1>Event Details Updated</h1>
            <p>Hello!</p>
            <p>The details of the event <strong>{eventEntity.Name}</strong> have been updated:</p>
            <ul>
                <li><strong>Start Date:</strong> {eventEntity.StartDateTime:g}</li>
                <li><strong>Location:</strong> {eventEntity.Location}</li>
            </ul>
            <p>Please review the updated details and adjust your plans accordingly.</p>
            <p>Thank you!</p>
        </body>
        </html>";
    }

    private async Task<Event> GetEventAndCheckIfItExists(Guid id, bool trackChanges, CancellationToken cancellationToken)
    {
        var eventEntity = await _repository.Event.GetEventByIdAsync(id, trackChanges, cancellationToken);
        if (eventEntity is null)
            throw new EventByIdNotFoundException(id);
        return eventEntity;
    }

    public async Task UploadImageAsync(
        Guid eventId, 
        bool trackChanges, 
        IFormFile imageFile, 
        CancellationToken cancellationToken)
    {
        if (imageFile?.Length > 1 * 1024 * 1024)
        {
            throw new HugeFileBadRequestException("1 MB");
        }

        using var transaction = _repository.BeginTransaction();

        try
        {
            var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);

            var oldFilePath = eventEntity.ImagePath;

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
            var fileName = await _fileService.SaveFileAsync(imageFile!, allowedFileExtentions);

            eventEntity.ImagePath = fileName;
            await _repository.SaveAsync(cancellationToken);

            if (!string.IsNullOrEmpty(oldFilePath))
            {
                _fileService.DeleteFile(oldFilePath);
            }
            _memoryCache.Remove($"EventImage_{eventId}");

            await _repository.CommitTransactionAsync(transaction, cancellationToken);

        }
        catch (Exception)
        {
            await _repository.RollbackTransactionAsync(transaction, cancellationToken);

            throw;
        }
    }

    public async Task DeleteImageAsync(Guid eventId, bool trackChanges, CancellationToken cancellationToken)
    {
        using var transaction = _repository.BeginTransaction();

        try
        {
            var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);
            var filePathToDelete = eventEntity.ImagePath;

            eventEntity.ImagePath = string.Empty;
            await _repository.SaveAsync(cancellationToken);

            _fileService.DeleteFile(filePathToDelete!);
            _memoryCache.Remove($"EventImage_{eventId}");

            await _repository.CommitTransactionAsync(transaction, cancellationToken);
        }
        catch (Exception)
        {
            await _repository.RollbackTransactionAsync(transaction, cancellationToken);

            throw;
        }
    }

    public async Task<(byte[] fileBytes, string contentType, string filename)> GetImageAsync(
        Guid eventId, bool trackChanges, CancellationToken cancellationToken)
    {
        var cacheKey = $"EventImage_{eventId}";
        if (_memoryCache.TryGetValue(cacheKey, out (byte[] fileBytes, string contentType, string filename) cachedResult))
        {
            return cachedResult;
        }
        var imageEvent = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);

        var (fileBytes, contentType, filename) = await _fileService.GetFileAsync(imageEvent.ImagePath!);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10)) 
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); 

        _memoryCache.Set(cacheKey, (fileBytes, contentType, filename), cacheOptions);

        return (fileBytes, contentType, filename);
    }
}

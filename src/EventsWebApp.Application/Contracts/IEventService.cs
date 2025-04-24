using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Domain.RequestFeatures;
using Microsoft.AspNetCore.Http;

namespace EventsWebApp.Application.Contracts;

public interface IEventService
{
    Task<(IEnumerable<EventDto> events, MetaData metaData)> GetAllEventAsync(
        EventParameters eventParameters, 
        bool trackChanges, 
        CancellationToken cancellationToken);

    Task<EventDto> GetEventByIdAsync(Guid eventId, bool trackChanges, CancellationToken cancellationToken);
    Task<EventDto> GetEventByNameAsync(string name, bool trackChanges, CancellationToken cancellationToken);
    Task<EventDto> CreateEventAsync(EventForCreationDto eventDto, CancellationToken cancellationToken);
    Task DeleteUserTaskAsync(Guid eventId, bool trackChanges, CancellationToken cancellationToken);

    Task UpdateUserTaskAsync(
        Guid eventId,
        EventForUpdateDto eventForUpdateDto,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task UploadImageAsync(
        Guid eventId,
        bool trackChanges,
        IFormFile imageFile,
        CancellationToken cancellationToken);

    Task DeleteImageAsync(Guid eventId, bool trackChanges, CancellationToken cancellationToken);

    Task<(byte[] fileBytes, string contentType, string filename)> GetImageAsync(
        Guid eventId, bool trackChanges, CancellationToken cancellationToken);
}

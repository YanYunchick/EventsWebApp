using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.RequestFeatures;

namespace EventsWebApp.Domain.Contracts;

public interface IEventRepository
{
    Task<PagedList<Event>> GetAllEventsAsync(
        EventParameters eventParameters,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<Event?> GetEventByIdAsync(
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<Event?> GetEventByNameAsync(
        string name,
        bool trackChanges,
        CancellationToken cancellationToken);

    void CreateEvent(Event eventEntity);
    void DeleteEvent(Event eventEntity);
}
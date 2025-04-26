using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.RequestFeatures;
using EventsWebApp.Infrastructure.Repository.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApp.Infrastructure.Repository.Repositories;

public class EventRepository : RepositoryBase<Event>, IEventRepository
{
    public EventRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Event>> GetAllEventsAsync(
        EventParameters eventParameters, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await FindAll(trackChanges)
                            .FilterEvent(
                                eventParameters.StartDateTime,
                                eventParameters.EndDateTime,
                                eventParameters.Location,
                                eventParameters.Category
                            )
                            .Search(eventParameters.SearchTerm!)
                            .OrderBy(x => x.Name)
                            .Include(e => e.UserEvents)
                            .ToPagedListAsync(
                                eventParameters.PageNumber, 
                                eventParameters.PageSize, 
                                cancellationToken);
    }

    public async Task<Event?> GetEventByIdAsync(
        Guid eventId, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await FindByCondition(ut => ut.Id.Equals(eventId), trackChanges)
                        .Include(e => e.UserEvents)
                        .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<Event?> GetEventByNameAsync(string name, bool trackChanges, CancellationToken cancellationToken)
    {
        return await FindByCondition(e => e.Name.Equals(name), trackChanges)
                        .Include(e => e.UserEvents)
                        .FirstOrDefaultAsync(cancellationToken);
    }

    public void CreateEvent(Event eventEntity) => Create(eventEntity);

    public void DeleteEvent(Event eventEntity) => Delete(eventEntity);
}

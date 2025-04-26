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

public class ParticipantUserRepository : RepositoryBase<User>, IParticipantUserRepository
{
    public ParticipantUserRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<User>> GetParticipantUsersByEventAsync(
        Guid eventId,
        UserParameters userParameters,
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await FindByCondition(u => u.UserEvents.Any(ue => ue.EventId.Equals(eventId)), trackChanges)
            .OrderBy(u => u.UserName)
            .Include(u => u.UserEvents.Where(ue => ue.EventId.Equals(eventId)))
            .ToPagedListAsync(userParameters.PageNumber,
                                userParameters.PageSize,
                                cancellationToken);
    }

    public async Task<User?> GetParticipantUserByIdsAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        return await FindByCondition(u => u.Id.Equals(userId), trackChanges)
            .Include(u => u.UserEvents.Where(ue => ue.EventId.Equals(eventId)))
            .SingleOrDefaultAsync(cancellationToken);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsWebApp.Infrastructure.Repository.Repositories;

public class UserEventRepository : RepositoryBase<UserEvent>, IUserEventRepository
{
    public UserEventRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<UserEvent?> GetUserEventByIdsAsync(
        string userId, 
        Guid eventId, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await FindByCondition(ue => ue.UserId.Equals(userId) && ue.EventId.Equals(eventId), trackChanges)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void CreateUserEvent(UserEvent userEvent) => Create(userEvent);
    public void DeleteUserEvent(UserEvent userEvent) => Delete(userEvent);
}

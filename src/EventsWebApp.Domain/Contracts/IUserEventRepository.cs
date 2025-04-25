using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Domain.Contracts;

public interface IUserEventRepository
{
    Task<UserEvent?> GetUserEventByIdsAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken);

    void CreateUserEvent(UserEvent userEvent);
    void DeleteUserEvent(UserEvent userEvent);
}

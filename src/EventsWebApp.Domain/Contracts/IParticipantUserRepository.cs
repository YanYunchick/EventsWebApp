using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.RequestFeatures;

namespace EventsWebApp.Domain.Contracts;

public interface IParticipantUserRepository
{
    Task<PagedList<User>> GetParticipantUsersByEventAsync(
        Guid eventId,
        UserParameters userParameters,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<User?> GetParticipantUserByIdsAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken);
}

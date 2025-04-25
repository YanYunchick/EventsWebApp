using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Domain.RequestFeatures;

namespace EventsWebApp.Application.Contracts;

public interface IParticipantUserService
{
    Task<(IEnumerable<ParticipantUserDto> participantUsers, MetaData metaData)> GetParticipantUsersByEventAsync(
        Guid eventId,
        UserParameters userParameters,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task<ParticipantUserDto> GetParticipantUserByIdsAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task CreateUserEventAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken);

    Task DeleteUserEventAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken);


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Exceptions;
using EventsWebApp.Domain.Models;
using EventsWebApp.Domain.RequestFeatures;
using Microsoft.AspNetCore.Identity;

namespace EventsWebApp.Application.Service;

internal sealed class ParticipantUserService : IParticipantUserService
{
    private readonly IRepositoryManager _repository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    
    public ParticipantUserService(IRepositoryManager repository, UserManager<User> userManager, IMapper mapper)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ParticipantUserDto> participantUsers, MetaData metaData)> GetParticipantUsersByEventAsync(
        Guid eventId,
        UserParameters userParameters,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);

        var usersWithMetaData = await _repository.User.GetParticipantUsersByEventAsync(eventId, userParameters, trackChanges, cancellationToken);

        var participantUsersDto = _mapper.Map<IEnumerable<ParticipantUserDto>>(usersWithMetaData);

        return (participantUsers: participantUsersDto, metaData: usersWithMetaData.MetaData);
    }

    public async Task<ParticipantUserDto> GetParticipantUserByIdsAsync(
        string userId,
        Guid eventId,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);

        var user = await _repository.User.GetParticipantUserByIdsAsync(userId, eventId, trackChanges, cancellationToken);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        var participantUserDto = _mapper.Map<ParticipantUserDto>(user);
        return participantUserDto;
    }

    public async Task CreateUserEventAsync(
        string userId, 
        Guid eventId, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        var eventEntity = await GetEventAndCheckIfItExists(eventId, trackChanges, cancellationToken);

        if (eventEntity.MaxParticipants <= eventEntity.UserEvents.Count())
        {
            throw new MaxParticipantsBadRequesException(eventId);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        var userEvent = new UserEvent()
        {
            UserId = userId,
            EventId = eventId,
            RegistrationDate = DateTime.UtcNow,
        };

        _repository.UserEvent.CreateUserEvent(userEvent);

        await _repository.SaveAsync(cancellationToken);
    }

    public async Task DeleteUserEventAsync(
        string userId, 
        Guid eventId, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        var userEvent = await _repository.UserEvent.GetUserEventByIdsAsync(userId, eventId, trackChanges, cancellationToken);
        if (userEvent == null)
        {
            throw new UserEventNotFoundException(userId, eventId);
        }

        _repository.UserEvent.DeleteUserEvent(userEvent);
        await _repository.SaveAsync(cancellationToken);
    }

    private async Task<Event> GetEventAndCheckIfItExists(Guid id, bool trackChanges, CancellationToken cancellationToken)
    {
        var eventEntity = await _repository.Event.GetEventByIdAsync(id, trackChanges, cancellationToken);
        if (eventEntity is null)
            throw new EventByIdNotFoundException(id);
        return eventEntity;
    }
    

}

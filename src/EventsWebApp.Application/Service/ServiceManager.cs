using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventsWebApp.Application.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IEventService> _eventService;
    private readonly Lazy<IParticipantUserService> _userService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(
        IRepositoryManager repositoryManager,
        IMapper mapper,
        IFileService fileService,
        UserManager<User> userManager,
        IConfiguration configuration,
        IEmailService emailService,
        IMemoryCache memoryCache)
    {
        _eventService = new Lazy<IEventService>(() => 
            new EventService(repositoryManager, mapper, fileService, emailService, memoryCache));
        _userService = new Lazy<IParticipantUserService>(() => 
            new ParticipantUserService(repositoryManager, userManager, mapper));
        _authenticationService = new Lazy<IAuthenticationService>(() =>
            new AuthenticationService(mapper, userManager, configuration));
    }

    public IEventService EventService => _eventService.Value;
    public IParticipantUserService ParticipantUserService => _userService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.Contracts;

namespace EventsWebApp.Application.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IEventService> _eventService;
    private readonly Lazy<IUserService> _userService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _eventService = new Lazy<IEventService>(() => new EventService(repositoryManager));
        _userService = new Lazy<IUserService>(() => new UserService(repositoryManager));
    }

    public IEventService EventService => _eventService.Value;
    public IUserService UserService => _userService.Value;

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Contracts;

public interface IServiceManager
{
    IEventService EventService { get; }
    IUserService UserService { get; }
    IAuthenticationService AuthenticationService { get; }
}

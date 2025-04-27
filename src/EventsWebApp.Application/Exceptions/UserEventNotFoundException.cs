using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class UserEventNotFoundException : NotFoundException
{
    public UserEventNotFoundException(string userId, Guid eventId)
        : base($"Connection between User ({userId}) and Event ({eventId}) doesn't exist.")
    {
    }
}

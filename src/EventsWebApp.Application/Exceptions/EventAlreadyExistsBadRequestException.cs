using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class EventAlreadyExistsBadRequestException : BadRequestException
{
    public EventAlreadyExistsBadRequestException(string eventName)
        : base($"The '{eventName}' event already exists.")
    {
        
    }
}

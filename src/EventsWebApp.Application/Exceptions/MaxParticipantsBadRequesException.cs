using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public class MaxParticipantsBadRequesException : BadRequestException
{
    public MaxParticipantsBadRequesException(Guid eventId)
        : base($"Event with id: {eventId} has maximum participants.")
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Exceptions;

public sealed class EventByIdNotFoundException : NotFoundException
{
    public EventByIdNotFoundException(Guid eventId)
        : base ($"The event with id: {eventId} doesn't exist in the database.")
    {

    }
}

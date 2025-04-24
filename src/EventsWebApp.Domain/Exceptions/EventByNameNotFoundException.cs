using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Exceptions;

public sealed class EventByNameNotFoundException : NotFoundException
{
    public EventByNameNotFoundException(string name)
    : base($"The event with name: '{name}' doesn't exist in the database.")
    {

    }
}

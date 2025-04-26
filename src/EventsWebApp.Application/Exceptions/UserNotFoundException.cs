using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string userId)
        : base($"User with id: {userId} doesn't exist in the database.")
    {
    }
}

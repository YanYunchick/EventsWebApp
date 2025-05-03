using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class InvalidCredentialsException : UnauthorizedException
{
    public InvalidCredentialsException()
        : base("Invalid login or password.")
    {
        
    }
}

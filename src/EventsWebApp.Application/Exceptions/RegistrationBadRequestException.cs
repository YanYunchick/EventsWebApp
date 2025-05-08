using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public class RegistrationBadRequestException : BadRequestException
{
    public IEnumerable<string> Errors { get; }
    public RegistrationBadRequestException(string message, IEnumerable<string> errors)
        : base(message)
    {
        Errors = errors;
    }
}

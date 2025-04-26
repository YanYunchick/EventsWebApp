using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class FileExtensionBadRequestException : BadRequestException
{
    public FileExtensionBadRequestException(string message)
        : base("Passed file extension is not allowed: " + message)
    {

    }
}

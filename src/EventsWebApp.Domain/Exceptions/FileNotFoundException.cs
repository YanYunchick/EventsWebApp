using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Exceptions;

public sealed class FileNotFoundException : NotFoundException
{
    public FileNotFoundException(string message)
        : base($"File not found: {message}")
    {
    }
}

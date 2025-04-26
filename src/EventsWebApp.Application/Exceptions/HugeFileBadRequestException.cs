using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class HugeFileBadRequestException : BadRequestException
{
    public HugeFileBadRequestException(string fileSizeLimit) 
        : base("File is larger than " + fileSizeLimit)
    {
    }
}

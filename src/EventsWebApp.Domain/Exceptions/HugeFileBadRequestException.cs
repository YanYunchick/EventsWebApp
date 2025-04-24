using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Exceptions;

public sealed class HugeFileBadRequestException : BadRequestException
{
    public HugeFileBadRequestException(string fileSizeLimit) 
        : base("File larger than " + fileSizeLimit)
    {
    }
}

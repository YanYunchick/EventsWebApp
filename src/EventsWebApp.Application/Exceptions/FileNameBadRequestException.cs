﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Exceptions;

public sealed class FileBadRequestException : BadRequestException
{
    public FileBadRequestException(string message) 
        : base(message)
    {
    }
}

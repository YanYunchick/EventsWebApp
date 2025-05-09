﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.DTOs.User;

public record ParticipantUserDto : UserDto
{
    public DateTime RegistrationDate { get; init; }
}

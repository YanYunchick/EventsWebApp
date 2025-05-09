﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace EventsWebApp.Application.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
    Task<TokenDto> Authenticate(UserForAuthenticationDto userForAuth);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}

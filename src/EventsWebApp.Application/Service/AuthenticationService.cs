using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Application.Exceptions;
using EventsWebApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EventsWebApp.Application.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public AuthenticationService(
        IMapper mapper, 
        UserManager<User> userManager,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = _mapper.Map<User>(userForRegistration);

        var result = await _userManager.CreateAsync(user, userForRegistration.Password!);

        if (result.Succeeded)
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles!);
        
        return result;
    }

    public async Task<TokenDto> Authenticate(UserForAuthenticationDto userForAuth)
    {
        var user = await _userManager.FindByNameAsync(userForAuth.UserName!);

        if(user == null && !await _userManager.CheckPasswordAsync(user!, userForAuth.Password!))
        {
            throw new InvalidCredentialsException();
        }

        return await _jwtService.CreateToken(user!);
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        return await _jwtService.RefreshToken(tokenDto);
    }
}

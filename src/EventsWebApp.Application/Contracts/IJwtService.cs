using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Application.Contracts;

public interface IJwtService
{
    Task<TokenDto> CreateToken(User user);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}

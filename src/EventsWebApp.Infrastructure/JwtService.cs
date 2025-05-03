using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Application.Exceptions;
using EventsWebApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EventsWebApp.Infrastructure;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public JwtService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<TokenDto> CreateToken(User user)
    {
        var accessToken = await GenerateAccessToken(user);

        var refreshToken = GenerateRefreshToken();

        user!.RefreshToken = refreshToken;

        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new TokenDto(accessToken, refreshToken);

    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);
        if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new RefreshTokenBadRequest();

        var accessToken = await GenerateAccessToken(user);
        return new TokenDto(accessToken, tokenDto.RefreshToken);
    }

    private async Task<string> GenerateAccessToken(User user)
    {
        var singingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(singingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user!.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials singingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
            (
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires").Value)),
                signingCredentials: singingCredentials
            );
        return tokenOptions;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!)),
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        return principal;
    }
}

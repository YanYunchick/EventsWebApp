using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApp.WebApi.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IServiceManager _service;

    public TokenController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
        var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenDto);
        return Ok(tokenDtoToReturn);
    }
}

using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.WebApi.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;
    public AuthenticationController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute<UserForRegistrationDto>))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        await _service.AuthenticationService.RegisterUser(userForRegistration);
        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
    {
        var tokenDto = await _service.AuthenticationService.Authenticate(user);

        return Ok(tokenDto);
    }
}

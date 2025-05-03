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
        var result = await _service.AuthenticationService.RegisterUser(userForRegistration);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }
        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
    {
        var tokenDto = await _service.AuthenticationService.Authenticate(user);

        return Ok(tokenDto);
    }
}

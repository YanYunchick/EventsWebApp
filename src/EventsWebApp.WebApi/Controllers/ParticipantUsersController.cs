using System.Security.Claims;
using System.Text.Json;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventsWebApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParticipantUsersController : ControllerBase
{
    private readonly IServiceManager _service;

    public ParticipantUsersController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "AdministratorOnly")]
    public async Task<IActionResult> GetParticipantUserByIds([FromQuery]Guid eventId, string id, CancellationToken cancellationToken)
    {
        var participant = await _service.ParticipantUserService
            .GetParticipantUserByIdsAsync(id, eventId, trackChanges: false, cancellationToken);
        return Ok(participant);
    }

    [HttpGet("event/{eventId:guid}")]
    [Authorize(Policy = "AdministratorOnly")]
    public async Task<IActionResult> GetParticipantUsersByEvent(
            Guid eventId,
            [FromQuery] UserParameters userParameters,
            CancellationToken cancellationToken)
    {
        var pagedResult = await _service.ParticipantUserService
            .GetParticipantUsersByEventAsync(eventId, userParameters, trackChanges: false, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.participantUsers);
    }

    [HttpPost("EventSubscription/{eventId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> SubscribeOnEvent(Guid eventId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _service.ParticipantUserService.CreateUserEventAsync(userId!, eventId, trackChanges: false, cancellationToken);

        return Created();
    }

    [HttpDelete("EventSubscription/{eventId:guid}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> UnsubscribeOnEvent(Guid eventId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _service.ParticipantUserService.DeleteUserEventAsync(userId!, eventId, trackChanges: false, cancellationToken);

        return NoContent();
    }
}

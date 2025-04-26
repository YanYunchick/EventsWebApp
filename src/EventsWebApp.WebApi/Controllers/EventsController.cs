using System.Text.Json;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Application.DTOs.File;
using EventsWebApp.Domain.RequestFeatures;
using EventsWebApp.WebApi.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EventsController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] EventParameters eventParameters, CancellationToken cancellationToken)
        {
            var result = await _service.EventService
                .GetAllEventAsync(eventParameters, trackChanges: false, cancellationToken);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.metaData));

            return Ok(result.events);
        }

        [HttpGet("{id:guid}", Name = "EventById")]
        public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken)
        {
            var eventDto = await _service.EventService.GetEventByIdAsync(id, trackChanges: false, cancellationToken);
            return Ok(eventDto);
        }

        [HttpPost]
        [Authorize(Policy = "AdministratorOnly")]
        [ServiceFilter(typeof(ValidationFilterAttribute<EventForManipulationDto>))]
        public async Task<IActionResult> CreateEvent([FromBody] EventForManipulationDto eventDto,
        CancellationToken cancellationToken)
        {
            var createdEvent = await _service.EventService.CreateEventAsync(eventDto, cancellationToken);
            return CreatedAtRoute("EventById", new { id = createdEvent.Id }, createdEvent);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteEvent(Guid id, CancellationToken cancellationToken)
        {
            await _service.EventService.DeleteEventAsync(id, trackChanges: false, cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdministratorOnly")]
        [ServiceFilter(typeof(ValidationFilterAttribute<EventForManipulationDto>))]
        public async Task<IActionResult> UpdateEvent(
            Guid id, 
            [FromBody] EventForManipulationDto eventDto,
            CancellationToken cancellationToken)
        {
            await _service.EventService.UpdateEventAsync(id, eventDto, trackChanges: true, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/image")]
        [Authorize(Policy = "AdministratorOnly")]
        [ServiceFilter(typeof(ValidationFilterAttribute<ImageUploadDto>))]
        public async Task<IActionResult> UploadEventImage(Guid id, [FromForm] ImageUploadDto model, CancellationToken cancellationToken)
        {
            await _service.EventService.UploadImageAsync(id, trackChanges: true, model.Image!, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}/image")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteEventImage(Guid id, CancellationToken cancellationToken)
        {
            await _service.EventService.DeleteImageAsync(id, trackChanges: true, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id:guid}/image")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> GetEventImage(Guid id, CancellationToken cancellationToken)
        {
            var image = await _service.EventService.GetImageAsync(id, trackChanges: false, cancellationToken);

            return File(
                image.fileBytes,
                image.contentType,
                image.filename);
        }
    }
}

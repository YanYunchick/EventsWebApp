using Microsoft.AspNetCore.Http;

namespace EventsWebApp.Application.DTOs.File;

public record ImageUploadDto
{
    public IFormFile? Image { get; set; }
}

using Microsoft.AspNetCore.Http;

namespace EventsWebApp.Application.DTOs.File;

public record FileUploadDto
{
    public IFormFile? File { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.File;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace EventsWebApp.Application.Validation.FileValidators;

public class ImageUploadDtoValidator : AbstractValidator<ImageUploadDto>
{
    private const int MaxImageSize = 1 * 1024 * 1024;
    public ImageUploadDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(dto => dto.Image)
            .NotNull()
            .WithMessage("Image cannot be null.")
            .Must(HasValidImageExtension!)
            .WithMessage("Invalid image format.")
            .Must(image => image!.Length < MaxImageSize)
            .WithMessage("Image is larger than " + MaxImageSize + " bytes");
    }

    private bool HasValidImageExtension(IFormFile image)
    {
        if (image.Length == 0)
            return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}

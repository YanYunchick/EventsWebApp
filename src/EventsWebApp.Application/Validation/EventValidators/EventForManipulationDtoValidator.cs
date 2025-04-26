using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.Event;
using FluentValidation;

namespace EventsWebApp.Application.Validation.EventValidators;

public class EventForManipulationDtoValidator : AbstractValidator<EventForManipulationDto>
{
    public EventForManipulationDtoValidator()
    {
        RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name must not exceed 255 characters.");

        RuleFor(dto => dto.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(dto => !string.IsNullOrEmpty(dto.Description));

        RuleFor(dto => dto.StartDateTime)
            .NotEmpty().WithMessage("StartDateTime is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("StartDateTime must be in the future.");

        RuleFor(dto => dto.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(255).WithMessage("Location must not exceed 255 characters.");

        RuleFor(dto => dto.Category)
            .IsInEnum().WithMessage("Category must be a valid value from the EventCategory enum.");

        RuleFor(dto => dto.MaxParticipants)
            .NotEmpty()
            .GreaterThan(0).WithMessage("MaxParticipants must be greater than 0.");
    }
}

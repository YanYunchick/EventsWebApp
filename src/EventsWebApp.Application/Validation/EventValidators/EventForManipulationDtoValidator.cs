using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Domain.Models;
using FluentValidation;

namespace EventsWebApp.Application.Validation.EventValidators;

public class EventForManipulationDtoValidator : AbstractValidator<EventForManipulationDto>
{
    private const int MaxNameLength = 255;
    private const int MaxDescriptionLength = 1000;
    private const int MaxLocationLength = 255;
    public EventForManipulationDtoValidator()
    {
        RuleFor(dto => dto.Name)
                .NotEmpty()
                    .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(EventForManipulationDto.Name)))
                .MaximumLength(MaxNameLength)
                    .WithMessage(ValidationUtility.TooLongFieldMessage(nameof(EventForManipulationDto.Name), MaxNameLength));

        RuleFor(dto => dto.Description)
            .MaximumLength(MaxDescriptionLength)
                .WithMessage(ValidationUtility.TooLongFieldMessage(nameof(EventForManipulationDto.Description), MaxDescriptionLength))
            .When(dto => !string.IsNullOrEmpty(dto.Description));

        RuleFor(dto => dto.StartDateTime)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(EventForManipulationDto.StartDateTime)))
            .GreaterThan(DateTime.UtcNow)
                .WithMessage(ValidationUtility.InFutureValueMessage(nameof(EventForManipulationDto.StartDateTime)));

        RuleFor(dto => dto.Location)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(EventForManipulationDto.Location)))
            .MaximumLength(MaxLocationLength)
                .WithMessage(ValidationUtility.TooLongFieldMessage(nameof(EventForManipulationDto.Location), MaxLocationLength));

        RuleFor(dto => dto.Category)
            .IsInEnum()
                .WithMessage(ValidationUtility.InEnumMessage(nameof(EventForManipulationDto.Category), nameof(EventCategory)));

        RuleFor(dto => dto.MaxParticipants)
            .NotEmpty()
            .GreaterThan(0)
                .WithMessage(ValidationUtility.PositiveValueMessage(nameof(EventForManipulationDto.MaxParticipants)));
    }
}

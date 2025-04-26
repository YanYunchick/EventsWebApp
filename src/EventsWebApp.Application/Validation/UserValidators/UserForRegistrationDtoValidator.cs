using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.DTOs.User;
using FluentValidation;

namespace EventsWebApp.Application.Validation.UserValidators;

public class UserForRegistrationDtoValidator : AbstractValidator<UserForRegistrationDto>
{
    public UserForRegistrationDtoValidator()
    {
        RuleFor(dto => dto.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters.");

        RuleFor(dto => dto.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(100).WithMessage("LastName must not exceed 100 characters.");

        RuleFor(dto => dto.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MinimumLength(3).WithMessage("UserName must be at least 3 characters long.")
            .MaximumLength(255).WithMessage("UserName must not exceed 255 characters.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.Birthdate)
            .NotEmpty().WithMessage("Birthdate is required.")
            .Must(HasValidDate).WithMessage("Birthdate must be a valid date and in the past.");
    }

    private bool HasValidDate(DateOnly? birthdate)
    {
        if (!birthdate.HasValue)
            return false;

        return birthdate.Value < DateOnly.FromDateTime(DateTime.UtcNow);
    }
}

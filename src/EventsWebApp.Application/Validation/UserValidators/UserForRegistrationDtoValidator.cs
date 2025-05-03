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
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;
    private const int MinUserNameLength = 3;
    private const int MaxUserNameLength = 255;
    private const int MinPasswordLength = 10;

    public UserForRegistrationDtoValidator()
    {
        RuleFor(dto => dto.FirstName)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(UserForRegistrationDto.FirstName)))
            .MaximumLength(MaxFirstNameLength)
                .WithMessage(ValidationUtility.TooLongFieldMessage(nameof(UserForRegistrationDto.FirstName), MaxFirstNameLength));

        RuleFor(dto => dto.LastName)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(UserForRegistrationDto.LastName)))
            .MaximumLength(MaxLastNameLength)
                .WithMessage(ValidationUtility.TooLongFieldMessage(nameof(UserForRegistrationDto.LastName), MaxLastNameLength));

        RuleFor(dto => dto.UserName)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(UserForRegistrationDto.UserName)))
            .MinimumLength(MinUserNameLength)
                .WithMessage(ValidationUtility.TooShortFieldMessage(nameof(UserForRegistrationDto.UserName), MinUserNameLength))
            .MaximumLength(MaxUserNameLength)
                .WithMessage(ValidationUtility.TooLongFieldMessage(nameof(UserForRegistrationDto.UserName), MaxUserNameLength));

        RuleFor(dto => dto.Password)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(UserForRegistrationDto.Password)))
            .MinimumLength(MinPasswordLength)
                .WithMessage(ValidationUtility.TooShortFieldMessage(nameof(UserForRegistrationDto.Password), MinPasswordLength))
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(dto => dto.Email)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(UserForRegistrationDto.Email)))
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.Birthdate)
            .NotEmpty()
                .WithMessage(ValidationUtility.RequiredFieldMessage(nameof(UserForRegistrationDto.Birthdate)))
            .Must(HasValidDate).WithMessage("Birthdate must be a valid date and in the past.");
    }

    private bool HasValidDate(DateOnly? birthdate)
    {
        if (!birthdate.HasValue)
            return false;

        return birthdate.Value < DateOnly.FromDateTime(DateTime.UtcNow);
    }
}

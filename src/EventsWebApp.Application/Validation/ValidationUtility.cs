using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Validation;

internal static class ValidationUtility
{
    internal static string RequiredFieldMessage(string fieldName)
    {
        return $"{fieldName} is required.";
    }

    internal static string TooLongFieldMessage(string fieldName, int length)
    {
        return $"{fieldName} must not exceed {length} characters.";
    }

    internal static string TooShortFieldMessage(string fieldName, int length)
    {
        return $"{fieldName} must be at least {length} characters long.";
    }

    internal static string InEnumMessage(string fieldName, string enumName)
    {
        return $"{fieldName} must be a valid value from the {enumName} enum.";
    }

    internal static string PositiveValueMessage(string fieldName)
    {
        return $"{fieldName} must be greater than 0.";
    }

    internal static string InFutureValueMessage(string fieldName)
    {
        return $"{fieldName} must be in the future.";
    }
}

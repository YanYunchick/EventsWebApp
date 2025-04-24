using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.DTOs.User;

public record UserForRegistrationDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? Email { get; init; }
    public DateOnly? Birthdate { get; init; } 
    public ICollection<string>? Roles { get; init; }

}

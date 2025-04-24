using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.DTOs.User;

public class UserForAuthenticationDto
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Models;

public class UserEvent
{
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }
}

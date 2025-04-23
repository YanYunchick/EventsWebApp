using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Models;


public enum EventCategory
{
    Concert = 1,
    Meetup = 2,
    Party = 3
}

public class Event
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public EventCategory Category { get; set; }
    public int MaxParticipants { get; set; }
    public ICollection<User>? Participants { get; set; } 
    public string? ImagePath { get; set; }

    public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Application.DTOs.Event;

public record EventDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime StartDateTime { get; init; } 
    public string? Location { get; init; }
    public EventCategory Category { get; init; }
    public int MaxParticipants { get; init; }
    public string? ImagePath { get; init; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace EventsWebApp.Application.DTOs.Event;

public abstract record EventForManipulationDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDateTime { get; init; }
    public string? Location { get; init; }
    public EventCategory Category { get; init; }
    public int MaxParticipants { get; init; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Domain.RequestFeatures;

public class EventParameters : RequestParameters
{
    public DateTime? StartDateTime { get; set; } = DateTime.MinValue;
    public DateTime? EndDateTime { get; set; } = DateTime.MaxValue;
    public string? Location { get; set; }
    public EventCategory? Category { get; set; }
    public string? SearchTerm { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace EventsWebApp.Infrastructure.Repository.Extensions;

public static class RepositoryEventExtensions
{
    public static IQueryable<Event> Search(this IQueryable<Event> events, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return events;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return events.Where(e => e.Name!.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Event> FilterEvent(
        this IQueryable<Event> events, 
        DateTime? startDateTime, 
        DateTime? endDateTime,
        string? location,
        EventCategory? category)
    {
        events = events.Where(e => e.StartDateTime >= startDateTime && e.StartDateTime <= endDateTime);
        if (category != null)
        {
            events = events.Where(e => e.Category == category);
        }
        if (!string.IsNullOrEmpty(location))
        {
            events = events.Where(e => e.Location!.ToLower().Contains(location!.Trim().ToLower()));
        }
        return events;
    }
        
}

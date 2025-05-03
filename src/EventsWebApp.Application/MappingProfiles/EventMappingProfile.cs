using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Application.MappingProfiles;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<Event, EventDto>();
        CreateMap<EventForManipulationDto, Event>().ReverseMap();
    }
}

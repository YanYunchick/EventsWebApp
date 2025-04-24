using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.DTOs.Event;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventDto>();
        CreateMap<EventForCreationDto, Event>();
        CreateMap<EventForUpdateDto, Event>().ReverseMap();
        CreateMap<UserForRegistrationDto, User>();

    }
}

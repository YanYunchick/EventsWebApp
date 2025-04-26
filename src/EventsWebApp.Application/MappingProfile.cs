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
        CreateMap<EventForManipulationDto, Event>();
        CreateMap<EventForManipulationDto, Event>().ReverseMap();

        CreateMap<UserForRegistrationDto, User>();

        CreateMap<User, ParticipantUserDto>()
            .ForMember(destination => destination.RegistrationDate,
                opts => opts
                    .MapFrom(src => src.UserEvents.First().RegistrationDate));

    }
}

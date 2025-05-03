using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.DTOs.User;
using EventsWebApp.Domain.Models;

namespace EventsWebApp.Application.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserForRegistrationDto, User>();

        CreateMap<User, ParticipantUserDto>()
            .ForMember(
                destination => destination.RegistrationDate,
                opts => opts.MapFrom(src => src.UserEvents.Any()
                    ? src.UserEvents.First().RegistrationDate
                    : DateTime.MinValue));
    }
}

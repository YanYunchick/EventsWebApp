using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.Contracts;

namespace EventsWebApp.Application.Service;

internal sealed class UserService : IUserService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public UserService(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
}

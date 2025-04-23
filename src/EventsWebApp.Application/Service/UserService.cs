using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.Contracts;

namespace EventsWebApp.Application.Service;

internal sealed class UserService : IUserService
{
    private readonly IRepositoryManager _repository;

    public UserService(IRepositoryManager repository)
    {
        _repository = repository;
    }
}

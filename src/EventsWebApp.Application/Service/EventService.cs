using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.Contracts;

namespace EventsWebApp.Application.Service;

internal sealed class EventService : IEventService
{
    private readonly IRepositoryManager _repository;

    public EventService(IRepositoryManager repository)
    {
        _repository = repository;
    }
}

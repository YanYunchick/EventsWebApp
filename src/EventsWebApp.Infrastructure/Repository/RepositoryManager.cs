using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Contracts;

namespace EventsWebApp.Infrastructure.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IEventRepository> _eventRepository;
    private readonly Lazy<IUserRepository> _userRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;

        _eventRepository = new Lazy<IEventRepository>(() => new
            EventRepository(repositoryContext));

        _userRepository = new Lazy<IUserRepository>(() => new
            UserRepository(repositoryContext));
    }

    public IEventRepository Event => _eventRepository.Value;
    public IUserRepository User => _userRepository.Value;

    public async Task SaveAsync(CancellationToken cancellationToken) => 
        await _repositoryContext.SaveChangesAsync(cancellationToken);
}

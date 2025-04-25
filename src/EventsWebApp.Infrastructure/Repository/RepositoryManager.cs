using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Infrastructure.Repository.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventsWebApp.Infrastructure.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IEventRepository> _eventRepository;
    private readonly Lazy<IParticipantUserRepository> _userRepository;
    private readonly Lazy<IUserEventRepository> _userEventRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;

        _eventRepository = new Lazy<IEventRepository>(() => new
            EventRepository(repositoryContext));

        _userRepository = new Lazy<IParticipantUserRepository>(() => new
            ParticipantUserRepository(repositoryContext));
        
        _userEventRepository = new Lazy<IUserEventRepository>(() => new
            UserEventRepository(repositoryContext));
    }

    public IEventRepository Event => _eventRepository.Value;
    public IParticipantUserRepository User => _userRepository.Value;
    public IUserEventRepository UserEvent => _userEventRepository.Value;

    public async Task SaveAsync(CancellationToken cancellationToken) => 
        await _repositoryContext.SaveChangesAsync(cancellationToken);

    public IDbContextTransaction BeginTransaction() =>
        _repositoryContext.Database.BeginTransaction();

    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken) =>
        await transaction.CommitAsync(cancellationToken);

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken) =>
        await transaction.RollbackAsync(cancellationToken);
}

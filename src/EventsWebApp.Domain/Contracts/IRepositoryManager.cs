using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventsWebApp.Domain.Contracts;

public interface IRepositoryManager
{
    IEventRepository Event { get; }
    IParticipantUserRepository User { get; }
    IUserEventRepository UserEvent { get; }

    Task SaveAsync(CancellationToken cancellationToken);
    IDbContextTransaction BeginTransaction();
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
}

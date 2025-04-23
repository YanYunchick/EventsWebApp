using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Domain.Contracts;

public interface IRepositoryManager
{
    IEventRepository Event { get; }
    IUserRepository User { get; }

    Task SaveAsync(CancellationToken cancellationToken);
}

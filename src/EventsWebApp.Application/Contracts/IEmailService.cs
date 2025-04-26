using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsWebApp.Application.Contracts;

public interface IEmailService
{
    Task SendEmailToManyAsync(IEnumerable<string> addresses, string subject, string? body);
}

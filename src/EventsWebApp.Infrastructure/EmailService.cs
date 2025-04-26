using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Application.Contracts;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace EventsWebApp.Infrastructure;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }
    public async Task SendEmailToManyAsync(IEnumerable<string> addresses, string subject, string? body)
    {
        IEnumerable<Address> fluentAddresses = addresses.Select(a => new Address { Name = a, EmailAddress = a});
        await _fluentEmail
            .To(fluentAddresses)
            .Subject(subject)
            .Body(body, isHtml:true)   
            .SendAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventsWebApp.Application.ErrorModel;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Details { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

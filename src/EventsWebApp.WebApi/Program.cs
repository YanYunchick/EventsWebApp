using EventsWebApp.Application.Contracts;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Infrastructure;
using EventsWebApp.Infrastructure.Repository;
using EventsWebApp.WebApi.ActionFilters;
using EventsWebApp.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureCors();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureAutoMapper();
builder.Services.AddTransient<IFileService, FileService>();

builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureAuthorizationPolicies();

builder.Services.AddControllers();
builder.Services.ConfigureSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.ConfigureFluentValidators();
builder.Services.AddScoped(typeof(ValidationFilterAttribute<>));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.ConfigureFluentEmail(builder.Configuration);
builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseAuthorization();
app.MapControllers();

app.Run();
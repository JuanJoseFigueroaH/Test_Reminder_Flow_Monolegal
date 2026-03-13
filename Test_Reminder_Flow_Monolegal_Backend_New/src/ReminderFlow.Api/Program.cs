using Microsoft.OpenApi.Models;
using ReminderFlow.Api.Middleware;
using ReminderFlow.Application.Clients.Commands;
using ReminderFlow.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Reminder Flow API",
        Version = "v1",
        Description = "API para gestión de recordatorios de facturas"
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateClientCommand).Assembly));
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminder Flow API v1");
        c.RoutePrefix = "docs";
    });
}

app.UseCors("AllowAll");
app.MapControllers();

Log.Information("Reminder Flow API iniciada en {Urls}", string.Join(", ", app.Urls));

app.Run();

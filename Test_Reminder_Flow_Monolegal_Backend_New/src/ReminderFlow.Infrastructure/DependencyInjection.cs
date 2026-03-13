using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Infrastructure.BackgroundServices;
using ReminderFlow.Infrastructure.Cache;
using ReminderFlow.Infrastructure.Email;
using ReminderFlow.Infrastructure.Persistence;
using ReminderFlow.Infrastructure.Repositories;
using ReminderFlow.Infrastructure.Settings;

namespace ReminderFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));

        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddHostedService<ReminderSchedulerService>();

        return services;
    }
}

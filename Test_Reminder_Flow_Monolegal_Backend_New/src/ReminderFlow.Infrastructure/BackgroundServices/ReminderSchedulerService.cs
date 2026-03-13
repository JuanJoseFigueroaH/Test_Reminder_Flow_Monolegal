using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Reminders.Commands;

namespace ReminderFlow.Infrastructure.BackgroundServices;

public class ReminderSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderSchedulerService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public ReminderSchedulerService(
        IServiceProvider serviceProvider,
        ILogger<ReminderSchedulerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Scheduler Service started - Processing reminders every {Interval} seconds", _interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing reminders");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Reminder Scheduler Service stopped");
    }

    private async Task ProcessRemindersAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduler: Starting automatic reminder processing...");

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var result = await mediator.Send(new ProcessRemindersCommand(), stoppingToken);
            
            if (result.Success && result.Data != null)
            {
                _logger.LogInformation(
                    "Scheduler: Processed {Total} reminders - {Success} successful, {Failed} failed",
                    result.Data.ProcessedCount,
                    result.Data.SuccessCount,
                    result.Data.FailedCount
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scheduler: Error processing reminders");
        }
    }
}

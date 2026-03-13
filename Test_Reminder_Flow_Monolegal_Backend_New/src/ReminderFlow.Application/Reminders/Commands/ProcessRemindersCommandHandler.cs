using System.Reactive.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Application.Reminders.Commands;

public class ProcessRemindersCommandHandler : IRequestHandler<ProcessRemindersCommand, ApiResponse<ReminderResultDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEmailService _emailService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProcessRemindersCommandHandler> _logger;

    public ProcessRemindersCommandHandler(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IEmailService emailService,
        ICacheService cacheService,
        ILogger<ProcessRemindersCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _emailService = emailService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<ReminderResultDto>> Handle(ProcessRemindersCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando procesamiento de recordatorios");

        var actionableStatuses = new[] 
        { 
            InvoiceStatus.Pendiente, 
            InvoiceStatus.PrimerRecordatorio, 
            InvoiceStatus.SegundoRecordatorio 
        };

        var invoices = await _invoiceRepository.GetByStatusAsync(actionableStatuses);
        var details = new List<ReminderDetailDto>();
        var successCount = 0;
        var failedCount = 0;

        var invoiceObservable = invoices.ToObservable();

        await invoiceObservable.ForEachAsync(async invoice =>
        {
            try
            {
                var client = await _clientRepository.GetByIdAsync(invoice.ClientId);
                if (client == null)
                {
                    _logger.LogWarning("Cliente no encontrado para factura: {InvoiceId}", invoice.Id);
                    details.Add(new ReminderDetailDto(
                        invoice.Id,
                        invoice.InvoiceNumber,
                        "Desconocido",
                        invoice.Status.ToDbString(),
                        invoice.Status.ToDbString(),
                        false,
                        "Cliente no encontrado"
                    ));
                    failedCount++;
                    return;
                }

                var previousStatus = invoice.Status;
                var newStatus = previousStatus.GetNextStatus();

                var emailSent = await _emailService.SendReminderEmailAsync(
                    client.Email,
                    client.Name,
                    invoice.InvoiceNumber,
                    invoice.Amount,
                    previousStatus.ToDbString(),
                    newStatus.ToDbString()
                );

                if (emailSent)
                {
                    invoice.Status = newStatus;
                    invoice.UpdatedAt = DateTime.UtcNow;
                    await _invoiceRepository.UpdateAsync(invoice.Id, invoice);

                    _logger.LogInformation(
                        "Recordatorio procesado: Factura {InvoiceNumber}, {PreviousStatus} -> {NewStatus}",
                        invoice.InvoiceNumber,
                        previousStatus.ToDbString(),
                        newStatus.ToDbString()
                    );

                    details.Add(new ReminderDetailDto(
                        invoice.Id,
                        invoice.InvoiceNumber,
                        client.Name,
                        previousStatus.ToDbString(),
                        newStatus.ToDbString(),
                        true,
                        null
                    ));
                    successCount++;
                }
                else
                {
                    details.Add(new ReminderDetailDto(
                        invoice.Id,
                        invoice.InvoiceNumber,
                        client.Name,
                        previousStatus.ToDbString(),
                        previousStatus.ToDbString(),
                        false,
                        "Error al enviar email"
                    ));
                    failedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando factura: {InvoiceId}", invoice.Id);
                details.Add(new ReminderDetailDto(
                    invoice.Id,
                    invoice.InvoiceNumber,
                    "Error",
                    invoice.Status.ToDbString(),
                    invoice.Status.ToDbString(),
                    false,
                    ex.Message
                ));
                failedCount++;
            }
        }, cancellationToken);

        await _cacheService.RemoveByPatternAsync("invoices:*");

        var result = new ReminderResultDto(
            details.Count,
            successCount,
            failedCount,
            details
        );

        _logger.LogInformation(
            "Procesamiento completado: {Total} procesados, {Success} exitosos, {Failed} fallidos",
            result.ProcessedCount,
            result.SuccessCount,
            result.FailedCount
        );

        return ApiResponse<ReminderResultDto>.Ok(result, "Recordatorios procesados exitosamente");
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Invoices.Commands;

public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, ApiResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteInvoiceCommandHandler> _logger;

    public DeleteInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICacheService cacheService,
        ILogger<DeleteInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando factura con ID: {Id}", request.Id);

        var deleted = await _invoiceRepository.DeleteAsync(request.Id);
        if (!deleted)
        {
            throw new NotFoundException("Factura", request.Id);
        }

        await _cacheService.RemoveByPatternAsync("invoices:*");

        _logger.LogInformation("Factura eliminada exitosamente: {Id}", request.Id);

        return ApiResponse.NoContent("Factura eliminada exitosamente");
    }
}

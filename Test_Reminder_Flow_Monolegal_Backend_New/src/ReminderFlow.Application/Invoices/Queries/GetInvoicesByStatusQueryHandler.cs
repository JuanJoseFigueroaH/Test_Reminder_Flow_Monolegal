using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Application.Invoices.Queries;

public class GetInvoicesByStatusQueryHandler : IRequestHandler<GetInvoicesByStatusQuery, ApiResponse<IEnumerable<InvoiceDto>>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<GetInvoicesByStatusQueryHandler> _logger;

    public GetInvoicesByStatusQueryHandler(
        IInvoiceRepository invoiceRepository,
        ILogger<GetInvoicesByStatusQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<InvoiceDto>>> Handle(GetInvoicesByStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo facturas por estado");

        var statuses = request.Statuses.Select(InvoiceStatusExtensions.FromDbString);
        var invoices = await _invoiceRepository.GetByStatusAsync(statuses);
        var invoiceDtos = invoices.Select(i => i.ToDto()).ToList();

        _logger.LogInformation("Se obtuvieron {Count} facturas", invoiceDtos.Count);

        return ApiResponse<IEnumerable<InvoiceDto>>.Ok(invoiceDtos);
    }
}

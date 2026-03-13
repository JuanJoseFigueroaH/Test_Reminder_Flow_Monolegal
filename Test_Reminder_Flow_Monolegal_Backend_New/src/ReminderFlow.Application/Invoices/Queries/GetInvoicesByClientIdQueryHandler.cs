using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Invoices.Queries;

public class GetInvoicesByClientIdQueryHandler : IRequestHandler<GetInvoicesByClientIdQuery, ApiResponse<IEnumerable<InvoiceDto>>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetInvoicesByClientIdQueryHandler> _logger;

    public GetInvoicesByClientIdQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICacheService cacheService,
        ILogger<GetInvoicesByClientIdQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<InvoiceDto>>> Handle(GetInvoicesByClientIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo facturas del cliente: {ClientId}", request.ClientId);

        var cacheKey = $"invoices:client:{request.ClientId}";
        var cachedInvoices = await _cacheService.GetAsync<IEnumerable<InvoiceDto>>(cacheKey);
        if (cachedInvoices != null)
        {
            _logger.LogInformation("Facturas del cliente obtenidas desde caché");
            return ApiResponse<IEnumerable<InvoiceDto>>.Ok(cachedInvoices);
        }

        var invoices = await _invoiceRepository.GetByClientIdAsync(request.ClientId);
        var invoiceDtos = invoices.Select(i => i.ToDto()).ToList();

        await _cacheService.SetAsync(cacheKey, invoiceDtos, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Se obtuvieron {Count} facturas del cliente", invoiceDtos.Count);

        return ApiResponse<IEnumerable<InvoiceDto>>.Ok(invoiceDtos);
    }
}

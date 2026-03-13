using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Invoices.Queries;

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, ApiResponse<InvoiceListResponseDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetAllInvoicesQueryHandler> _logger;
    private const string CacheKey = "invoices:all";

    public GetAllInvoicesQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICacheService cacheService,
        ILogger<GetAllInvoicesQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<InvoiceListResponseDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo todas las facturas");

        var cachedResponse = await _cacheService.GetAsync<InvoiceListResponseDto>(CacheKey);
        if (cachedResponse != null)
        {
            _logger.LogInformation("Facturas obtenidas desde caché");
            return ApiResponse<InvoiceListResponseDto>.Ok(cachedResponse);
        }

        var invoices = await _invoiceRepository.GetAllAsync();
        var invoiceDtos = invoices.Select(i => i.ToDto()).ToList();
        var response = new InvoiceListResponseDto(invoiceDtos, invoiceDtos.Count);

        await _cacheService.SetAsync(CacheKey, response, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Se obtuvieron {Count} facturas", invoiceDtos.Count);

        return ApiResponse<InvoiceListResponseDto>.Ok(response);
    }
}

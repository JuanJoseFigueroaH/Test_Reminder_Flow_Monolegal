using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Invoices.Queries;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, ApiResponse<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetInvoiceByIdQueryHandler> _logger;

    public GetInvoiceByIdQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICacheService cacheService,
        ILogger<GetInvoiceByIdQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo factura con ID: {Id}", request.Id);

        var cacheKey = $"invoices:{request.Id}";
        var cachedInvoice = await _cacheService.GetAsync<InvoiceDto>(cacheKey);
        if (cachedInvoice != null)
        {
            _logger.LogInformation("Factura obtenida desde caché: {Id}", request.Id);
            return ApiResponse<InvoiceDto>.Ok(cachedInvoice);
        }

        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice == null)
        {
            throw new NotFoundException("Factura", request.Id);
        }

        var invoiceDto = invoice.ToDto();
        await _cacheService.SetAsync(cacheKey, invoiceDto, TimeSpan.FromMinutes(5));

        return ApiResponse<InvoiceDto>.Ok(invoiceDto);
    }
}

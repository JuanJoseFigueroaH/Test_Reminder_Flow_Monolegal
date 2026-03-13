using MediatR;
using Microsoft.Extensions.Logging;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Mappers;
using ReminderFlow.Domain.Exceptions;
using ReminderFlow.Domain.Ports;

namespace ReminderFlow.Application.Invoices.Commands;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, ApiResponse<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        ICacheService cacheService,
        ILogger<CreateInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando nueva factura: {InvoiceNumber}", request.InvoiceNumber);

        var client = await _clientRepository.GetByIdAsync(request.ClientId);
        if (client == null)
        {
            throw new NotFoundException("Cliente", request.ClientId);
        }

        var invoice = new CreateInvoiceDto(
            request.ClientId,
            request.InvoiceNumber,
            request.Amount,
            request.DueDate,
            request.Status
        ).ToEntity();

        var createdInvoice = await _invoiceRepository.CreateAsync(invoice);

        await _cacheService.RemoveByPatternAsync("invoices:*");

        _logger.LogInformation("Factura creada exitosamente con ID: {Id}", createdInvoice.Id);

        return ApiResponse<InvoiceDto>.Created(createdInvoice.ToDto(), "Factura creada exitosamente");
    }
}

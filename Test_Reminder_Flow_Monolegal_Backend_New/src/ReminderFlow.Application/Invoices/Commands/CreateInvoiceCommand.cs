using MediatR;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;

namespace ReminderFlow.Application.Invoices.Commands;

public record CreateInvoiceCommand(
    string ClientId,
    string InvoiceNumber,
    decimal Amount,
    DateTime? DueDate,
    string? Status
) : IRequest<ApiResponse<InvoiceDto>>;

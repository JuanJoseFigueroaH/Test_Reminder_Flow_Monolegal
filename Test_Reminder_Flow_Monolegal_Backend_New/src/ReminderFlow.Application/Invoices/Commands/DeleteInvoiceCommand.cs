using MediatR;
using ReminderFlow.Application.Common;

namespace ReminderFlow.Application.Invoices.Commands;

public record DeleteInvoiceCommand(string Id) : IRequest<ApiResponse>;

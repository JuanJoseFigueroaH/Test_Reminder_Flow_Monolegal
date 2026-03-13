using MediatR;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;

namespace ReminderFlow.Application.Invoices.Queries;

public record GetInvoicesByClientIdQuery(string ClientId) : IRequest<ApiResponse<IEnumerable<InvoiceDto>>>;

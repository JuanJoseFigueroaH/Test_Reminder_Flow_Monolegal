using MediatR;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;

namespace ReminderFlow.Application.Invoices.Queries;

public record GetInvoicesByStatusQuery(IEnumerable<string> Statuses) : IRequest<ApiResponse<IEnumerable<InvoiceDto>>>;

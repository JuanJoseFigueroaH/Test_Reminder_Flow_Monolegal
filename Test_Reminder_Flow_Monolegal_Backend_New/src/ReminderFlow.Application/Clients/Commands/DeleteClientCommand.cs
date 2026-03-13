using MediatR;
using ReminderFlow.Application.Common;

namespace ReminderFlow.Application.Clients.Commands;

public record DeleteClientCommand(string Id) : IRequest<ApiResponse>;

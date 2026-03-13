using MediatR;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;

namespace ReminderFlow.Application.Clients.Commands;

public record UpdateClientCommand(string Id, string Name, string Email, string Phone) : IRequest<ApiResponse<ClientDto>>;

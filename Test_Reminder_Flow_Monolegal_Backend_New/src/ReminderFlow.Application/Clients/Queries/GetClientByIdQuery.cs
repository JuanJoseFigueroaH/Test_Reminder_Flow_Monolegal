using MediatR;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;

namespace ReminderFlow.Application.Clients.Queries;

public record GetClientByIdQuery(string Id) : IRequest<ApiResponse<ClientDto>>;

using MediatR;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;

namespace ReminderFlow.Application.Reminders.Commands;

public record ProcessRemindersCommand : IRequest<ApiResponse<ReminderResultDto>>;

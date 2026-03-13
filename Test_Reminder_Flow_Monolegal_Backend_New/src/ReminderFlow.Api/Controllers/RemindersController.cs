using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReminderFlow.Application.Common;
using ReminderFlow.Application.DTOs;
using ReminderFlow.Application.Reminders.Commands;

namespace ReminderFlow.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RemindersController : ControllerBase
{
    private readonly IMediator _mediator;

    public RemindersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("process")]
    [ProducesResponseType(typeof(ApiResponse<ReminderResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessReminders()
    {
        var result = await _mediator.Send(new ProcessRemindersCommand());
        return StatusCode(result.StatusCode, result);
    }
}

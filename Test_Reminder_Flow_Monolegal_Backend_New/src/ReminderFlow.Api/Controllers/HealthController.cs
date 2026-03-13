using Microsoft.AspNetCore.Mvc;
using ReminderFlow.Application.Common;

namespace ReminderFlow.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(ApiResponse<object>.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
    }
}

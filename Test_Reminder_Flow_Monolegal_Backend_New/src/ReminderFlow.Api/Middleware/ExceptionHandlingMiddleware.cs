using System.Net;
using System.Text.Json;
using ReminderFlow.Application.Common;
using ReminderFlow.Domain.Exceptions;

namespace ReminderFlow.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = exception switch
        {
            NotFoundException notFoundEx => new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundEx.Message,
                Data = null
            },
            ValidationException validationEx => new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = validationEx.Message,
                Data = null
            },
            BusinessException businessEx => new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.UnprocessableEntity,
                Message = businessEx.Message,
                Data = null
            },
            DomainException domainEx => new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = domainEx.Message,
                Data = null
            },
            _ => new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Error interno del servidor",
                Data = null
            }
        };

        if (apiResponse.StatusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Excepción controlada: {Message}", exception.Message);
        }

        response.StatusCode = apiResponse.StatusCode;
        await response.WriteAsync(JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

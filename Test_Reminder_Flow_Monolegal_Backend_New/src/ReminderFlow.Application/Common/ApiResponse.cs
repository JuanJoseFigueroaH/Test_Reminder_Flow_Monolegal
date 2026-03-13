namespace ReminderFlow.Application.Common;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public bool Success => StatusCode >= 200 && StatusCode < 300;

    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            StatusCode = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Created(T data, string message = "Recurso creado exitosamente")
    {
        return new ApiResponse<T>
        {
            StatusCode = 201,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> NotFound(string message = "Recurso no encontrado")
    {
        return new ApiResponse<T>
        {
            StatusCode = 404,
            Message = message,
            Data = default
        };
    }

    public static ApiResponse<T> BadRequest(string message = "Solicitud inválida")
    {
        return new ApiResponse<T>
        {
            StatusCode = 400,
            Message = message,
            Data = default
        };
    }

    public static ApiResponse<T> Error(string message = "Error interno del servidor", int statusCode = 500)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Message = message,
            Data = default
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string message = "Operación exitosa")
    {
        return new ApiResponse
        {
            StatusCode = 200,
            Message = message,
            Data = null
        };
    }

    public static ApiResponse NoContent(string message = "Sin contenido")
    {
        return new ApiResponse
        {
            StatusCode = 204,
            Message = message,
            Data = null
        };
    }
}

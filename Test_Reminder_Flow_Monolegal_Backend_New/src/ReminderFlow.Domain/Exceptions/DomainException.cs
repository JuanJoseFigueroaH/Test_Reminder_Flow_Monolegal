namespace ReminderFlow.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, string id) 
        : base("NOT_FOUND", $"{entity} con ID '{id}' no fue encontrado")
    {
    }
}

public class ValidationException : DomainException
{
    public ValidationException(string message) 
        : base("VALIDATION_ERROR", message)
    {
    }
}

public class BusinessException : DomainException
{
    public BusinessException(string message) 
        : base("BUSINESS_ERROR", message)
    {
    }
}

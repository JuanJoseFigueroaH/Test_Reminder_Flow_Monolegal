namespace ReminderFlow.Application.DTOs;

public record ReminderResultDto(
    int ProcessedCount,
    int SuccessCount,
    int FailedCount,
    List<ReminderDetailDto> Details
);

public record ReminderDetailDto(
    string InvoiceId,
    string InvoiceNumber,
    string ClientName,
    string PreviousStatus,
    string NewStatus,
    bool EmailSent,
    string? Error
);

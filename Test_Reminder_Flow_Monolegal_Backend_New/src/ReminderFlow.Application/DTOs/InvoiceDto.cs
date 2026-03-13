using System.Text.Json.Serialization;

namespace ReminderFlow.Application.DTOs;

public record InvoiceDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("invoice_number")] string InvoiceNumber,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("due_date")] DateTime? DueDate,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

public record InvoiceListResponseDto(
    [property: JsonPropertyName("invoices")] IEnumerable<InvoiceDto> Invoices,
    [property: JsonPropertyName("total")] int Total
);

public record CreateInvoiceDto(
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("invoice_number")] string InvoiceNumber,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("due_date")] DateTime? DueDate,
    [property: JsonPropertyName("status")] string? Status
);

public record UpdateInvoiceDto(
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("invoice_number")] string InvoiceNumber,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("due_date")] DateTime? DueDate,
    [property: JsonPropertyName("status")] string Status
);

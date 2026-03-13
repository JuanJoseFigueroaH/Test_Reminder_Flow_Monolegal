using System.Text.Json.Serialization;

namespace ReminderFlow.Application.DTOs;

public record ClientDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("phone")] string Phone,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
);

public record ClientListResponseDto(
    [property: JsonPropertyName("clients")] IEnumerable<ClientDto> Clients,
    [property: JsonPropertyName("total")] int Total
);

public record CreateClientDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("phone")] string Phone
);

public record UpdateClientDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("phone")] string Phone
);

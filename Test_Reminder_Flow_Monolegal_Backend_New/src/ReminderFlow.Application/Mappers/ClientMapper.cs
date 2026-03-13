using ReminderFlow.Application.DTOs;
using ReminderFlow.Domain.Entities;

namespace ReminderFlow.Application.Mappers;

public static class ClientMapper
{
    public static ClientDto ToDto(this Client client)
    {
        return new ClientDto(
            client.Id,
            client.Name,
            client.Email,
            client.Phone,
            client.CreatedAt,
            client.UpdatedAt
        );
    }

    public static Client ToEntity(this CreateClientDto dto)
    {
        return new Client
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone
        };
    }

    public static void UpdateEntity(this Client client, UpdateClientDto dto)
    {
        client.Name = dto.Name;
        client.Email = dto.Email;
        client.Phone = dto.Phone;
        client.UpdatedAt = DateTime.UtcNow;
    }
}

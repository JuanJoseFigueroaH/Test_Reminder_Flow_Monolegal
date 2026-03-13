using MongoDB.Bson;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.ValueObjects;
using ReminderFlow.Infrastructure.Persistence.Documents;

namespace ReminderFlow.Infrastructure.Persistence.Mappers;

public static class DocumentMapper
{
    public static Client ToEntity(this ClientDocument document)
    {
        return new Client
        {
            Id = document.Id.ToString(),
            Name = document.Name,
            Email = document.Email,
            Phone = document.Phone,
            CreatedAt = document.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = document.UpdatedAt
        };
    }

    public static ClientDocument ToDocument(this Client entity)
    {
        return new ClientDocument
        {
            Id = string.IsNullOrEmpty(entity.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(entity.Id),
            Name = entity.Name,
            Email = entity.Email,
            Phone = entity.Phone,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Invoice ToEntity(this InvoiceDocument document)
    {
        return new Invoice
        {
            Id = document.Id.ToString(),
            ClientId = document.ClientId,
            InvoiceNumber = document.InvoiceNumber,
            Amount = document.Amount,
            DueDate = document.DueDate,
            Status = InvoiceStatusExtensions.FromDbString(document.Status),
            CreatedAt = document.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = document.UpdatedAt
        };
    }

    public static InvoiceDocument ToDocument(this Invoice entity)
    {
        return new InvoiceDocument
        {
            Id = string.IsNullOrEmpty(entity.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(entity.Id),
            ClientId = entity.ClientId,
            InvoiceNumber = entity.InvoiceNumber,
            Amount = entity.Amount,
            DueDate = entity.DueDate,
            Status = entity.Status.ToDbString(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

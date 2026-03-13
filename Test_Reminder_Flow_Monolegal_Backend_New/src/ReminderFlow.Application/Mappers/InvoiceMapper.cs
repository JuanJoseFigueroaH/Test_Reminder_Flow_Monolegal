using ReminderFlow.Application.DTOs;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Application.Mappers;

public static class InvoiceMapper
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id,
            invoice.ClientId,
            invoice.InvoiceNumber,
            invoice.Amount,
            invoice.DueDate,
            invoice.Status.ToDbString(),
            invoice.CreatedAt,
            invoice.UpdatedAt
        );
    }

    public static Invoice ToEntity(this CreateInvoiceDto dto)
    {
        return new Invoice
        {
            ClientId = dto.ClientId,
            InvoiceNumber = dto.InvoiceNumber,
            Amount = dto.Amount,
            DueDate = dto.DueDate,
            Status = string.IsNullOrEmpty(dto.Status) 
                ? InvoiceStatus.Pendiente 
                : InvoiceStatusExtensions.FromDbString(dto.Status)
        };
    }

    public static void UpdateEntity(this Invoice invoice, UpdateInvoiceDto dto)
    {
        invoice.ClientId = dto.ClientId;
        invoice.InvoiceNumber = dto.InvoiceNumber;
        invoice.Amount = dto.Amount;
        invoice.DueDate = dto.DueDate;
        invoice.Status = InvoiceStatusExtensions.FromDbString(dto.Status);
        invoice.UpdatedAt = DateTime.UtcNow;
    }
}

using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Domain.Entities;

public class Invoice : BaseEntity
{
    public string ClientId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pendiente;
}

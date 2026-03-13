using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.ValueObjects;

namespace ReminderFlow.Domain.Ports;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(string id);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(string clientId);
    Task<IEnumerable<Invoice>> GetByStatusAsync(IEnumerable<InvoiceStatus> statuses);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice?> UpdateAsync(string id, Invoice invoice);
    Task<bool> DeleteAsync(string id);
}

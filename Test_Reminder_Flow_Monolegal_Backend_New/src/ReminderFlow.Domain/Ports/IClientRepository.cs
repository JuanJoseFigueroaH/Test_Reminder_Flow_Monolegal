using ReminderFlow.Domain.Entities;

namespace ReminderFlow.Domain.Ports;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(string id);
    Task<Client> CreateAsync(Client client);
    Task<Client?> UpdateAsync(string id, Client client);
    Task<bool> DeleteAsync(string id);
}

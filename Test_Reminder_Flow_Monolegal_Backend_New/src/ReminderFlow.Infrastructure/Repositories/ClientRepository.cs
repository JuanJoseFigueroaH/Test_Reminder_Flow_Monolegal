using MongoDB.Bson;
using MongoDB.Driver;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Infrastructure.Persistence;
using ReminderFlow.Infrastructure.Persistence.Documents;
using ReminderFlow.Infrastructure.Persistence.Mappers;

namespace ReminderFlow.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IMongoCollection<ClientDocument> _collection;

    public ClientRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<ClientDocument>("clients");
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(d => d.ToEntity());
    }

    public async Task<Client?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var document = await _collection.Find(c => c.Id == objectId).FirstOrDefaultAsync();
        return document?.ToEntity();
    }

    public async Task<Client> CreateAsync(Client client)
    {
        var document = new ClientDocument
        {
            Id = ObjectId.GenerateNewId(),
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            CreatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
        return document.ToEntity();
    }

    public async Task<Client?> UpdateAsync(string id, Client client)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var update = Builders<ClientDocument>.Update
            .Set(c => c.Name, client.Name)
            .Set(c => c.Email, client.Email)
            .Set(c => c.Phone, client.Phone)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<ClientDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(c => c.Id == objectId, update, options);
        return document?.ToEntity();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;

        var result = await _collection.DeleteOneAsync(c => c.Id == objectId);
        return result.DeletedCount > 0;
    }
}

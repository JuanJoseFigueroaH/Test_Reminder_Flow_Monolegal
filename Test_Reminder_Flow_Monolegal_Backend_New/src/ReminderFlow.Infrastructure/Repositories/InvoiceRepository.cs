using MongoDB.Bson;
using MongoDB.Driver;
using ReminderFlow.Domain.Entities;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Domain.ValueObjects;
using ReminderFlow.Infrastructure.Persistence;
using ReminderFlow.Infrastructure.Persistence.Documents;
using ReminderFlow.Infrastructure.Persistence.Mappers;

namespace ReminderFlow.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IMongoCollection<InvoiceDocument> _collection;

    public InvoiceRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<InvoiceDocument>("invoices");
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(d => d.ToEntity());
    }

    public async Task<Invoice?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var document = await _collection.Find(i => i.Id == objectId).FirstOrDefaultAsync();
        return document?.ToEntity();
    }

    public async Task<IEnumerable<Invoice>> GetByClientIdAsync(string clientId)
    {
        if (!ObjectId.TryParse(clientId, out _))
            return Enumerable.Empty<Invoice>();

        var documents = await _collection.Find(i => i.ClientId == clientId).ToListAsync();
        return documents.Select(d => d.ToEntity());
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(IEnumerable<InvoiceStatus> statuses)
    {
        var statusStrings = statuses.Select(s => s.ToDbString()).ToList();
        var filter = Builders<InvoiceDocument>.Filter.In(i => i.Status, statusStrings);
        var documents = await _collection.Find(filter).ToListAsync();
        return documents.Select(d => d.ToEntity());
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        var document = new InvoiceDocument
        {
            Id = ObjectId.GenerateNewId(),
            ClientId = invoice.ClientId,
            InvoiceNumber = invoice.InvoiceNumber,
            Amount = invoice.Amount,
            DueDate = invoice.DueDate,
            Status = invoice.Status.ToDbString(),
            CreatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
        return document.ToEntity();
    }

    public async Task<Invoice?> UpdateAsync(string id, Invoice invoice)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var update = Builders<InvoiceDocument>.Update
            .Set(i => i.ClientId, invoice.ClientId)
            .Set(i => i.InvoiceNumber, invoice.InvoiceNumber)
            .Set(i => i.Amount, invoice.Amount)
            .Set(i => i.DueDate, invoice.DueDate)
            .Set(i => i.Status, invoice.Status.ToDbString())
            .Set(i => i.UpdatedAt, DateTime.UtcNow);

        var options = new FindOneAndUpdateOptions<InvoiceDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(i => i.Id == objectId, update, options);
        return document?.ToEntity();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;

        var result = await _collection.DeleteOneAsync(i => i.Id == objectId);
        return result.DeletedCount > 0;
    }
}

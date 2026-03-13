using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReminderFlow.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
public class InvoiceDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("client_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClientId { get; set; } = string.Empty;

    [BsonElement("invoice_number")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [BsonElement("amount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Amount { get; set; }

    [BsonElement("due_date")]
    [BsonIgnoreIfNull]
    public DateTime? DueDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("created_at")]
    [BsonIgnoreIfNull]
    public DateTime? CreatedAt { get; set; }

    [BsonElement("updated_at")]
    [BsonIgnoreIfNull]
    public DateTime? UpdatedAt { get; set; }
}

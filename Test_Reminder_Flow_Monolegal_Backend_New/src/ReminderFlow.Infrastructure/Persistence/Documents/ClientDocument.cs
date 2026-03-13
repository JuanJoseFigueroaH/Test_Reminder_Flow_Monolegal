using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReminderFlow.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
public class ClientDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("created_at")]
    [BsonIgnoreIfNull]
    public DateTime? CreatedAt { get; set; }

    [BsonElement("updated_at")]
    [BsonIgnoreIfNull]
    public DateTime? UpdatedAt { get; set; }
}

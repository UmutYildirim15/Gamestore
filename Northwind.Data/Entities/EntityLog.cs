using MongoDB.Bson;

namespace Northwind.Data.Entities;

public class EntityLog : BaseEntity
{
    public DateTime Date { get; set; }
    public required string Action { get; set; }
    public string? EntityType { get; set; }
    public BsonDocument? Entity { get; set; }
    public BsonDocument? UpdEntity { get; set; }
}
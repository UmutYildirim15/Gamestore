namespace Northwind.Data.Entities;

public class Category : BaseEntity
{
    public long CategoryId { get; set; }

    public required string CategoryName { get; set; }

    public string? Description { get; set; }

    public string? Picture { get; set; }

    public string? ParentId { get; set; }
}
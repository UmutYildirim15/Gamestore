namespace Northwind.Data.Entities
{
    public class Product : BaseEntity
    {
        public long ProductId { get; set; }

        public required string ProductName { get; set; }

        public required string Key { get; set; }

        public long SupplierId { get; set; }

        public long CategoryId { get; set; }

        public string? QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }

        public short UnitsInStock { get; set; }

        public uint UnitsOnOrder { get; set; }

        public uint ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public int CommentCount { get; set; }
        public int Popularity { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Deleted { get; set; }

        public string? Description { get; set; }
    }
}


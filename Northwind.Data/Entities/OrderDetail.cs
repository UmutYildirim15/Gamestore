using MongoDB.Bson.Serialization.Attributes;

namespace Northwind.Data.Entities
{
    public class OrderDetail : BaseEntity
    {
        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public float Discount { get; set; }
    }
}
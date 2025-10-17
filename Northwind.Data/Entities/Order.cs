namespace Northwind.Data.Entities;

public class Order : BaseEntity
{
    public long OrderId { get; set; }

    public string? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public IList<OrderDetail>? OrderDetails { get; set; }
}
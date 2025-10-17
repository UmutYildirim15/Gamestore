namespace Data.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public required string Method { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double Amount { get; set; }
    public Order? Order { get; set; }
}
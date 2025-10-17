namespace Shared.Dto;

public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime Date { get; set; }
}

public class OrderDetailResponse
{
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double Discount { get; set; }
}

public class CartResponse
{
    public List<OrderDetailResponse> Items { get; set; } = new();
}

public class PaymentMethodsResponse
{
    public List<PaymentMethod> PaymentMethods { get; set; } = new();
}

public class PaymentMethod
{
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}


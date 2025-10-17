namespace Web.Models.CreateModels;

public class OrderGameCreate
{
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double Discount { get; set; } = 0;
}
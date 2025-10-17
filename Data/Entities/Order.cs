namespace Data.Entities
{
    public enum OrderStatus
    {
        Open,      
        Checkout,   
        Paid,       
        Cancelled,
        Shipped   
    }

    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Open;
        public List<OrderGame> OrderGames { get; set; } = new List<OrderGame>();
    }
}


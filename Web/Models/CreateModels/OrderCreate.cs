namespace Web.Models.CreateModels
{
    public class OrderCreate
    {
        public Guid CustomerId { get; set; } 
        public List<OrderGameCreate> OrderGames { get; set; } = new();
    }
}
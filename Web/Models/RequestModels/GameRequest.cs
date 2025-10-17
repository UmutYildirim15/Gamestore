namespace Web.Models.RequestModels
{
    public class GameRequest
    {
        public Guid? Id { get; set; }
        public required string Name { get; set; }
        public required string Key { get; set; }
        public required double Price { get; set; }
        public required int UnitInStock { get; set; }
        public required int Discount { get; set; }
        public string? Description { get; set; }
    }
}
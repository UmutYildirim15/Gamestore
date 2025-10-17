namespace Northwind.Data.Entities
{
    public class Shipper : BaseEntity
    {
        public required long ShipperID { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
    }
}
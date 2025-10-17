namespace Northwind.Data.Entities
{
    public class Supplier : BaseEntity
    {
        public long SupplierId { get; set; }

        public required string CompanyName { get; set; }

        public string? ContactTitle { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public string? Phone { get; set; }

        public string? Fax { get; set; }

        public string? HomePage { get; set; }
        
        public string? Description { get; set; }
    }
}
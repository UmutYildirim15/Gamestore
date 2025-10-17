namespace Data.Entities
{
    public class Publisher
    {
        public virtual Guid Id { get; set; }
        public virtual required string CompanyName { get; set; }
        public virtual string? ContactTitle { get; set; }
        public virtual string? Address { get; set; }
        public virtual string? City { get; set; }
        public virtual string? Region { get; set; }
        public virtual string? PostalCode { get; set; }
        public virtual string? Country { get; set; }
        public virtual string? Phone { get; set; }
        public virtual string? Fax { get; set; }
        public virtual string? HomePage { get; set; }
        public virtual string? Description { get; set; }
    }
}
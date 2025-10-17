namespace Data.Entities
{
    public class Platform
    {
        public virtual Guid Id { get; set; }
        public virtual required string Type { get; set; }
    }
}

namespace Web.Models.RequestModels
{
    public class PlatformRequest
    {
        public Guid? Id { get; set; } 
        public required string Type { get; set; }
    }
}

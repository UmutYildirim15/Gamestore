namespace Web.Models.RequestModels
{
    public class PublisherRequest
    {
        public required string CompanyName { get; set; }
        public string? HomePage { get; set; }
        public string? Description { get; set; }
    }
}
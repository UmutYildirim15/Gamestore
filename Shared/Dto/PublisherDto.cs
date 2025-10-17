namespace Shared.Dto;

public class PublisherResponse
{
    public Guid Id { get; set; }
    public required string CompanyName { get; set; }
    public  string? HomePage { get; set; }
    public  string? Description { get; set; }
}
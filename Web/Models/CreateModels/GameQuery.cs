namespace Web.Models.CreateModels;

public class GameQuery
{
    public List<string>? Genres { get; set; }
    public List<string>? Platforms { get; set; }
    public List<string>? Publishers { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public string? PublishDateOption { get; set; }
    public string? NamePart { get; set; }
    public string? SortingOption { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
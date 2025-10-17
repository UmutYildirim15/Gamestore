namespace Shared.Dto;

public class GameFilterRequest
{
    public List<string>? Genres             { get; set; }
    public List<string>? Platforms          { get; set; }
    public List<string>? Publishers         { get; set; }
    public double?       MinPrice           { get; set; }
    public double?       MaxPrice           { get; set; }
    public string?       PublishDateOption  { get; set; }
    public string?       NamePart           { get; set; }
    public string?       SortingOption      { get; set; }

    public int Page     { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items       { get; set; } = Array.Empty<T>();
    public int              TotalItems  { get; set; }
    public int              Page        { get; set; }
    public int              PageSize    { get; set; }
    public int              TotalPages  
        => (int)Math.Ceiling((double)TotalItems / PageSize);
}
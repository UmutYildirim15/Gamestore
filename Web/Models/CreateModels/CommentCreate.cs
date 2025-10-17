namespace Web.Models.CreateModels;

public class CommentCreate
{ 
    public Guid? ParentId { get; set; } 
    public string? Action { get; set; }
    public required string Name { get; set; }
    public required string Body { get; set; }
}
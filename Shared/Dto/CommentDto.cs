namespace Shared.Dto
{
    public class CommentRequest
    {
        public CommentDto Comment { get; set; } = null!;

        public Guid? ParentId { get; set; }

        public string? Action { get; set; }
    }

    public class CommentDto
    {
        public required string Name { get; set; }
        public required string Body { get; set; }
    }

    public class CommentResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Body { get; set; }
        public List<CommentResponse> ChildComments { get; set; } = new List<CommentResponse>();
    }
}
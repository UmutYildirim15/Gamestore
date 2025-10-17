namespace Web.Models.RequestModels
{
    public class RoleRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<string> Permissions { get; set; } = [];
    }
}
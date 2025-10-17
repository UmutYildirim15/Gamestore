namespace Web.Models.CreateModels
{
    public class RoleCreate
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public List<string> Permissions { get; set; } = [];
    }
}
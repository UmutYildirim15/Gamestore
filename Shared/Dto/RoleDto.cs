namespace Shared.Dto
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<string> Permissions { get; set; } = new List<string>();
    }
}
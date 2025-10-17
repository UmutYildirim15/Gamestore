namespace Shared.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public IEnumerable<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
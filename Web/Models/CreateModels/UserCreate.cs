namespace Web.Models.CreateModels
{
    public class UserCreate
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public List<Guid> RoleIds { get; set; } = [];
    }
}
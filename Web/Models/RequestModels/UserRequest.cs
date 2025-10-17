namespace Web.Models.RequestModels
{
    public class UserRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public List<Guid> RoleIds { get; set; } = [];
    }
}
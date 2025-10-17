using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
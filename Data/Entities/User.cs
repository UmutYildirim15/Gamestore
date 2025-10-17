using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
    
        [Required]
        [MaxLength(255)]
        public required string Password { get; set; }

        public List<UserRole> UserRoles { get; set; } = new();
    }
}
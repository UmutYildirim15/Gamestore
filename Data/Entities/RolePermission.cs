namespace Data.Entities;

public class RolePermission
{
    public required Guid RoleId { get; set; }
    public required Role Role { get; set; }

    public required Guid PermissionId { get; set; }
    public required Permission Permission { get; set; }
}

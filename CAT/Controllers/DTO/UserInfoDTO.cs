namespace CAT.Controllers.DTO
{
    public class UserInfoDTO
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? OrganizationId { get; set; }

        public string RoleId { get; set; } = null!;

        public string? OrganizationName { get; set; }

        public string[] PermissionIds { get; set; } = null!;
    }
}

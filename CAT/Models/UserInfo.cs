namespace CAT.Models
{
    public class UserInfo
    {
        public Guid Id { get; set; }

        public Guid? OrganizationId { get; set; }

        public Guid RoleId { get; set; }

        public string? OrganizationName { get; set; }

        public Guid[] PermissionIds { get; set; }
    }
}

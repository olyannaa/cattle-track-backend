namespace CAT.EF.DAL;

public partial class GroupType
{
    public Guid Id { get; set; }

    public Guid? OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<Group>? Groups { get; set; }
}
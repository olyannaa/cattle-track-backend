using System.ComponentModel.DataAnnotations.Schema;

namespace CAT.EF.DAL;

public partial class Research
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("animal_id")]
    public Guid? AnimalId { get; set; }

    [Column("organization_id")]
    public Guid? OrganizationId { get; set; }

    [Column("research_name")]
    public string? Name { get; set; }

    [Column("material_type")]
    public string? MaterialType { get; set; }

    [Column("collection_date")]
    public DateTime? CollectionDate { get; set; }

    [Column("collected_by")]
    public DateTime? CollectedBy { get; set; }

    [Column("result")]
    public string? Result { get; set;}

    [Column("notes")]
    public string? Notes { get; set;}

    [Column("created_at")]
    public DateTime? CreatedAt { get; set;}
}
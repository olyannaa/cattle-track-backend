using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public partial class ActiveAnimalDAL
{
    [Column("animal_id")]
    public Guid Id { get; set; }

    [Column("tag_number")] 
    public string TagNumber { get; set; } = null!;

    [Column("type")]
    public string? Type { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("group_name")]
    public string? GroupName { get; set; }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CAT.Controllers.DTO
{
    [NotMapped]
    [ExcludeFromCodeCoverage]
    public class CowInseminationDTO
    {
        [Column("organization_id")]
        public Guid OrganizationId { get; set; }

        [Column("cow_id")]
        public Guid CowId { get; set; }
        [Column("cow_tag_number")]
        public string? CowTagNumber{ get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("insemination_type")]
        public string? InseminationType { get; set; }

        [Column("insemination_date")]
        public DateTime? InseminationDate { get; set; }

        [Column("bull_id")]
        public Guid? BullId { get; set; } = Guid.Empty;
        [Column("bull_tag_number")]
        public string? BullTagNumber { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
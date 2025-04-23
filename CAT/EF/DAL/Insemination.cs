using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAT.EF.DAL
{
    [Table("insemination")]
    public class Insemination
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("cow_id")]
        public Guid? CowId { get; set; }
        [Column("bull_id")]
        public Guid? BullId { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("insemination_type")]
        public string? Type { get; set; }
        [Column("sperm_batch")]
        public string? SpermBatch { get; set; }
        [Column("sperm_manufacturer")]
        public string? SpermManufacturer { get; set; }
        [Column("embryo_id")]
        public Guid? EmbryoId { get; set; }
        [Column("embryo_manufacturer")]
        public string? EmbryoManufacturer { get; set; }

        [Column("technician")]
        public string? Technician { get; set; }
        [Column("notes")]
        public string? Notes { get; set; }


        [ForeignKey("cow_id")]
        public virtual Animal? Cow{ get; set; }
        [ForeignKey("bull_id")]
        public virtual Animal? Bull { get; set; }
    }
}

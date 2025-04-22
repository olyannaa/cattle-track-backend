using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAT.EF.DAL
{
    [Table("pregnancy")]
    public class Pregnancy
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("cow_id")]
        public Guid CowId { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("expected_date")]
        public DateTime? ExpectedDate { get; set; }

        [ForeignKey("cow_id")]
        public virtual Animal? Cow { get; set; }
    }
}

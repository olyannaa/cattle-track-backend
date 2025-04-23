

using System.ComponentModel.DataAnnotations.Schema;

namespace CAT.EF.DAL
{
    public partial class AnimalCensus
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tag_number")]
        public string TagNumber { get; set; } = null!;

        [Column("birth_date")]
        public DateOnly? BirthDate { get; set; }

        [Column("breed")]
        public string? Breed { get; set; }

        [Column("group_name")]
        public string? GroupName { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("origin")]
        public string? Origin { get; set; }

        [Column("origin_location")]
        public string? OriginLocation { get; set; }

        [Column("mother_tag_number")]
        public string? MotherTagNumber { get; set; }

        [Column("father_tag_number")]
        public string? FatherTagNumber { get; set; }
    }
}

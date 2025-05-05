

using System.ComponentModel.DataAnnotations.Schema;

namespace CAT.EF.DAL
{
    public partial class AnimalCensus
    {
        [Column("animal_id")]
        public Guid Id { get; set; }

        [Column("tag_number")]
        public string TagNumber { get; set; } = null!;

        [Column("birth_date")]
        public DateOnly? BirthDate { get; set; }

        [Column("type")]
        public string? Type { get; set; }

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

        [Column("date_of_receipt")]
        public DateOnly? DateOfReceipt { get; set; }

        [Column("date_of_disposal")]
        public DateOnly? DateOfDisposal { get; set; }

        [Column("reason_of_disposal")]
        public string? ReasonOfDisposal { get; set; }

        [Column("consumption")]
        public string? Consumption { get; set; }

        [Column("live_weight_at_disposal")]
        public double? LiveWeightAtDisposal { get; set; }

        [Column("last_weigh_date")]
        public DateOnly? LastWeightDate { get; set; }

        [Column("last_weight_weight")]
        public string? LastWeightWeight { get; set; }

        [Column("identification_field_name")]
        public string? IdentificationFieldName { get; set; }

        [Column("identification_value")]
        public string? IdentificationValue { get; set; }
    }
}

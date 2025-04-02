using CAT.EF.DAL;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class AnimalRegistrationDTO
    {
        [Required]
        public string TagNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [Required]
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Status { get; set; }
        public string? MotherTag { get; set; }
        public Guid OrganizationId { get; set; }
        public string? FatherTag { get; set; }
        public Guid? GroupId { get; set; }
        public string Origin { get; set; }
        public string OriginLocation { get; set; }
        public IFormFile? Photo { get; set; }
        public Dictionary<string, string>? AdditionalFields { get; set; } = new();

        //if netel
        [Format("dd.MM.yyyy")]
        public DateOnly InseminationDate { get; set; } = new DateOnly();
        [Format("dd.MM.yyyy")]
        public DateOnly ExpectedCalvingDate { get; set; } = new DateOnly();
        public string InseminationType { get; set; } = null;
        public string SpermBatch { get; set; } = null;
        public string Technician { get; set; } = null;
        public string Notes { get; set; } = null;
    }
}

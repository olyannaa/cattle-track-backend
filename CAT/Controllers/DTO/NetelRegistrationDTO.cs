using CsvHelper.Configuration.Attributes;

namespace CAT.Controllers.DTO
{
    public class NetelRegistrationDTO : AnimalRegistrationDTO
    {
        [Format("dd.MM.yyyy")]
        public DateOnly InseminationDate { get; set; }
        [Format("dd.MM.yyyy")]
        public DateOnly ExpectedCalvingDate { get; set; }
        public string InseminationType { get; set; }
        public string SpermBatch { get; set; }
        public string Technician { get; set; }
        public string Notes { get; set; }
    }
}

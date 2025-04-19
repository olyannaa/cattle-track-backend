using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class VaccinationActionDTO : DailyActionDTO
    {
        public string? Type { get; init; }

        public DateTime? NextActionDate { get; init; }

        public string? Medicine { get; init; }

        public string? Dose { get; init; }
    }
}
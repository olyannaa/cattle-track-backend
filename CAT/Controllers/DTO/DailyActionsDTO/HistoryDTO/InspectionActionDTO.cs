using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class InspectionActionDTO : DailyActionDTO
    {
        public string? Type { get; init; }

        public DateTime? NextActionDate { get; init; }

        public string? Result { get; init; }
    }
}
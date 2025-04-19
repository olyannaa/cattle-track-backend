using System.ComponentModel.DataAnnotations;
using CAT.Logic;

namespace CAT.Controllers.DTO
{
    public class GetDailyActionsDTO
    {
        [Required]
        public string? Type { get; init; }

        [Required, GreaterThan(0)]
        public int Page { get; init; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class GetDailyActionsDTO
    {
        public string? Type { get; init; }

        public int Page { get; init; }
    }
}
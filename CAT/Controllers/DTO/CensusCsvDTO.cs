using System.ComponentModel.DataAnnotations;
using CAT.Logic;

namespace CAT.Controllers.DTO
{
    public class CensusCsvDTO
    {
        /// <summary>
        /// Тип животного
        /// </summary>
        /// <example>Корова</example>
        [Required]
        [IsIn("Корова", "Бык", "Бычок", "Нетель", "Телка")]
        public string Type { get; init; }

        public CensusSortInfoDTO SortInfo { get; init; }
    }
}

using System.ComponentModel.DataAnnotations;
using CAT.Logic;

namespace CAT.Controllers.DTO
{
    public class CensusQueryDTO
    {
        /// <summary>
        /// Тип животного
        /// </summary>
        /// <example>Корова</example>
        [Required]
        public string Type { get; init; }

        /// <summary>
        /// Номер страницы
        /// </summary>
        /// <example>Корова</example>
        [Required, GreaterThan(0)]
        public int Page { get; init; }

        /// <summary>
        /// Отображать ли только активных животных
        /// </summary>
        /// <example>true</example>
        public bool? Active { get; init; }
    }
}

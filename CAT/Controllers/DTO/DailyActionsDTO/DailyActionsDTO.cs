using System.ComponentModel.DataAnnotations;
using CAT.Logic;

namespace CAT.Controllers.DTO
{
    public class DailyActionsDTO
    {
        /// <summary>
        /// Тип ежедневного действия
        /// </summary>
        /// <example>Осмотры</example>
        [Required]
        [IsIn("Осмотры", "Вакцинации и обработки", "Лечение", "Перевод", "Выбытие", "Исследования", "Присвоение номеров")]
        public string? Type { get; init; }

        /// <summary>
        /// Номер страницы
        /// </summary>
        /// <example>1</example>
        [Required, GreaterThan(0)]
        public int Page { get; init; }
    }
}
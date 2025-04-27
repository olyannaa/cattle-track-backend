using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class PaginationQueryDTO
    {
        /// <summary>
        /// Тип животного
        /// </summary>
        /// <example>Корова</example>
        public string Type { get; init; }

        /// <summary>
        /// Учитывать ли только активных животных
        /// </summary>
        /// <example>true</example>
        public bool? Active { get; init; }
    }
}

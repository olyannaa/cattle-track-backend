
using CAT.Logic;

namespace CAT.Controllers.DTO;
public class CensusSortInfoDTO
{
        /// <summary>
        /// Отображать ли только активных животных
        /// </summary>
        /// <example>true</example>
        public bool Active { get; init; } = default;

        /// <summary>
        /// Название колонки по сортировке
        /// </summary>
        /// <example>TagNumber</example>
        [IsIn("TagNumber", "BirthDate", "Breed", "GroupName", "Status", "Origin",
                "OriginLocation", "MotherTagNumber", "FatherTagNumber", "DateOfReceipt",
                "DateOfDisposal", "ReasonOfDisposal", "Consumption", "LiveWeihtAtDisposal",
                "LastWeightDate", "LastWeightWeight", "IdentificationFieldName", "IdentificationValue")]
        public string? Column { get; init; } = default;

        /// <summary>
        /// Сортировать ли по убыванию
        /// </summary>
        /// <example>true</example>
        public bool Descending { get; init; } = default;
}
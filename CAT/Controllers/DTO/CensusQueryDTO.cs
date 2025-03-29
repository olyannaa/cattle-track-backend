using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class CensusQueryDTO
    {
        /// <summary>
        /// ID организации
        /// </summary>
        /// <example>e97a7dbf-7336-44b8-bb3f-407fc59ecf2b</example>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Тип животного
        /// </summary>
        /// <example>Корова</example>
        [Required]
        public string Type { get; set; }
    }
}

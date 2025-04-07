using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class CensusQueryDTO
    {
        /// <summary>
        /// Тип животного
        /// </summary>
        /// <example>Корова</example>
        [Required]
        public string Type { get; set; }
    }
}

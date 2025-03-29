using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class CensusRouteDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Type { get; set; }
    }
}

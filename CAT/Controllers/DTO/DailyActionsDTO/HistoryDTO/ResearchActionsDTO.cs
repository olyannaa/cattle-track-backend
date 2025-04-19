using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class ResearchActionDTO
    {
        public Guid Id { get; init; }

        public string? TagNumber { get; init; } = null!;

        public DateTime? PerformDate { get; init; }

        public string? PerformedBy { get; init; }

        public string? MaterialType { get; init; }
    
        public string? Notes { get; init; }

        public DateTime? ResultDate { get; init; }
    }
}
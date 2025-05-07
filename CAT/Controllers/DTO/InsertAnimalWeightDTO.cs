namespace CAT.Controllers.DTO
{
    public class InsertAnimalWeightDTO
    {
            public Guid Id { get; set; }
            public Guid CalfId { get; set; }
            public DateOnly Date { get; set; }
            public double Weight { get; set; }
            public string Method { get; set; }
            public string? Notes { get; set; }
    }
}

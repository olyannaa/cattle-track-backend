namespace CAT.Controllers.DTO
{
    public class TreatmentActionDTO : DailyActionDTO
    {
        public string? Result { get; init; }

        public DateTime? NextActionDate { get; init; }

        public string? Medicine { get; init; }

        public string? Dose { get; init; }
    }
}
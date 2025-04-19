namespace CAT.Controllers.DTO
{
    public class GetDailyAnimalsDTO
    {
        public Guid? GroupId { get; init; }

        public string? Type { get; init; }

        public string? TagNumber { get; init; }

        public Guid? IdentificationId { get; set; }
    }
}
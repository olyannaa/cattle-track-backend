namespace CAT.Controllers.DTO
{
    public class UpdateAnimalDTO
    {
        public string? TagNumber { get; init; }

        public string? Type { get; init; }

        public Guid? GroupID { get; init; }

        public DateOnly? BirthDate { get; init; }

        public string? Status { get; init; }
    }
}

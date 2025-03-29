namespace CAT.Controllers.DTO
{
    public class UpdateAnimalDTO
    {
        /// <example>TAG123</example>
        public string? TagNumber { get; init; }

        /// <example>Бык</example>
        public string? Type { get; init; }

        /// <example>be7d9e62-9163-43fa-98e5-6ce7a2665317</example>
        public Guid? GroupID { get; init; }

        /// <example>2023-12-31</example>
        public DateOnly? BirthDate { get; init; }

        /// <example>Alive</example>
        public string? Status { get; init; }
    }
}

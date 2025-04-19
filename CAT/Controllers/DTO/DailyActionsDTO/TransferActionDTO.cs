using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class TransferActionDTO : DailyActionDTO
    {
        public string? Type { get; init; }

        public Guid? OldGroupId { get; init; }

        public string? OldGroupName { get; init; }

        public Guid? NewGroupId { get; init; }

        public string? NewGroupName { get; init; }
    }
}

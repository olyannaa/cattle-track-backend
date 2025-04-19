using CAT.Controllers.DTO;

namespace CAT.Services.Interfaces
{
    public interface IDailyActionService
    {
        public IEnumerable<dynamic> GetDailyActions(Guid organizationId, string type);
        public IEnumerable<dynamic> GetDailyActionsByPage(Guid organizationId, string type,
                                                             int page = 1, bool isMoblile = default);
    }
}
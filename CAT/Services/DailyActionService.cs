using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Services.Interfaces;

namespace CAT.Services
{
    public class DailyActionService : IDailyActionService
    {
        private readonly PostgresContext _db;

        public DailyActionService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public IEnumerable<dynamic> GetDailyActions(Guid organizationId, string type)
        {
            return _db.GetDailyActions(organizationId, type);
        }

        public IEnumerable<dynamic>? GetDailyActionsByPage(Guid organizationId, string type, int page = 1, bool isMoblile = default)
        {
            var (skip, take) = ComputePagination(isMoblile, page);
            return _db.GetDailyActionsWithPagination(organizationId, type, skip, take);
        }

        public void DeleteDailyAction(Guid dailyActionId)
        {
            var dailyAction = _db.DailyActions.SingleOrDefault(e => e.Id == dailyActionId);

            if (dailyAction == null)
            {
                var research = _db.Researches.SingleOrDefault(e => e.Id == dailyActionId);

                if (research == null)
                    throw new Exception(message: $"Ошибка. Не удалось удалить ежедневное действие, т.к его не существует.");

                _db.Researches.Remove(research);
                _db.SaveChanges();
                return;
            }
                
            _db.DailyActions.Remove(dailyAction);
            _db.SaveChanges();
        }

        private static (int skip, int take) ComputePagination(bool isMobile, int page)
        {
            var take = isMobile ? 5 : 10;
            var skip = (page - 1) * take;
            return (skip, take);
        }
    }
}
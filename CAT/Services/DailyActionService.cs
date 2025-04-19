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

        public IEnumerable<AnimalCensus> GetAnimalCensus(Guid organizationId, string animalType)
        {
            return _db.GetAnimalsByOrgAndType(organizationId, animalType);
        }

        public IEnumerable<AnimalCensus> GetAnimalCensusByPage(Guid organizationId, string animalType, int page = 1, bool isMoblile = default)
        {
            var (skip, take) = ComputePagination(isMoblile, page);
            return _db.GetAnimalsWithPagintaion(organizationId, animalType, skip, take);
        }

        public IEnumerable<dynamic> GetDailyActions(Guid organizationId, string type)
        {
            return _db.GetDailyActions(organizationId, type);
        }

        public IEnumerable<dynamic> GetDailyActionsByPage(Guid organizationId, string type, int page = 1, bool isMoblile = default)
        {
            var (skip, take) = ComputePagination(isMoblile, page);
            return _db.GetDailyActionsWithPagination(organizationId, type, skip, take);
        }

        private static (int skip, int take) ComputePagination(bool isMobile, int page)
        {
            var take = isMobile ? 5 : 10;
            var skip = (page - 1) * take;
            return (skip, take);
        }
    }
}
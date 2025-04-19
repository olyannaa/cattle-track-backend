using CAT.EF;
using CAT.EF.DAL;
using CAT.Services.Interfaces;

namespace CAT.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly PostgresContext _db;

        public AnimalService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public IEnumerable<AnimalCensus> GetAnimalCensus(Guid organizationId, string animalType)
        {
            return _db.GetAnimalsByOrgAndType(organizationId, animalType);
        }

        public IEnumerable<ActiveAnimalDAL> GetActiveAnimals(Guid organizationId)
        {
            return _db.GetActiveAnimals(organizationId);
        }

        public IEnumerable<AnimalCensus> GetAnimalCensusByPage(Guid organizationId, string animalType, int page = 1, bool isMoblile = default)
        {
            var take = isMoblile ? 5 : 10;
            var skip = (page - 1) * take;
            return _db.GetAnimalsWithPagintaion(organizationId, animalType, skip, take);
        }
    }
}

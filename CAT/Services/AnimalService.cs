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

        public IEnumerable<AnimalCensus> GetAnimalCensus(Guid organisationId, string animalType)
        {
            return _db.GetAnimalsByOrgAndType(organisationId, animalType);
        }
    }
}

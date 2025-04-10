using CAT.EF.DAL;

namespace CAT.Services.Interfaces
{
    public interface IAnimalService
    {
        IEnumerable<AnimalCensus> GetAnimalCensus(Guid organisationId, string animalType);

        IEnumerable<AnimalCensus> GetAnimalCensusByPage(Guid organisationId, string animalType, int page = 1, bool isMoblile = default);
    }
}

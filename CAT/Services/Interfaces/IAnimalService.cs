using CAT.EF.DAL;

namespace CAT.Services.Interfaces
{
    public interface IAnimalService
    {
        IEnumerable<AnimalCensus> GetAnimalCensus(Guid organisationId, string animalType);
    }
}

using CAT.Controllers.DTO;
using CAT.EF.DAL;
using CAT.Models;

namespace CAT.Services.Interfaces
{
    public interface IAnimalService
    {
        void RegisterAnimal(AnimalRegistrationDTO animal, Guid organizationId);
        void UpdateAnimal(UpdateAnimalDTO updateInfo);
        List<GroupInfoDTO>? GetGroupsInfo(Guid org_id);
        List<IdentificationInfoDTO>? GetIdentificationsFields(Guid org_id);
        public ImportAnimalsInfo ImportAnimalsFromCSV(List<AnimalCSVInfoDTO> animals, Guid org_id);
        IEnumerable<AnimalCensus> GetAnimalCensus(Guid organisationId, string animalType, CensusSortInfoDTO sortInfo);
        IEnumerable<AnimalCensus> GetAnimalCensusByPage(Guid organisationId, string animalType, CensusSortInfoDTO sortInfo, int page = 1, bool isMoblile = default);
        IEnumerable<CowDTO> GetCows(Guid organizationId);
        IEnumerable<BullDTO> GetBulls(Guid organizationId);
        void InsertInsemination(InseminationDTO dto);
        void InsertPregnancy(PregnancyDTO dto);
        void InsertCalving(CalvingDTO dto);
        IEnumerable<CowInseminationDTO> GetPregnancies(Guid organizationId);
        void InsertPregnancy(InsertPregnancyDTO dto);
        void InsertCalving(InsertCalvingDTO dto);
    }
}

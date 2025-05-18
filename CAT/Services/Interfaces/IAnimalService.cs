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
        IEnumerable<AnimalDTO> GetAnimalCensus(Guid organisationId, string animalType, CensusSortInfoDTO sortInfo);
        IEnumerable<AnimalDTO> GetAnimalCensusByPage(Guid organisationId, string animalType, CensusSortInfoDTO sortInfo, int page = 1, bool isMoblile = default);
        Dictionary<string, int> GetMainPageInfo(Guid organizationId);
        IEnumerable<CowDTO> GetCows(Guid organizationId);
        IEnumerable<BullDTO> GetBulls(Guid organizationId);
        void InsertInsemination(InseminationDTO dto);
        void InsertCalving(CalvingDTO dto);
        IEnumerable<CowInseminationDTO> GetPregnanciesForInsert(Guid organizationId);
        IEnumerable<CowInseminationDTO> GetPregnanciesForCalving(Guid organizationId);
        void InsertPregnancy(InsertPregnancyDTO dto);
        Guid InsertCalving(InsertCalvingDTO dto, Guid organizationId);
    }
}

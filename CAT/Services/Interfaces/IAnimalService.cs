using CAT.Controllers.DTO;
using CAT.EF.DAL;
using CAT.Models;

namespace CAT.Services.Interfaces
{
    public interface IAnimalService
    {
        void RegisterAnimal(AnimalRegistrationDTO animal);
        List<GroupInfoDTO>? GetGroupsInfo(Guid org_id);
        List<IdentificationInfoDTO>? GetIdentificationsFields(Guid org_id);
        public ImportAnimalsInfo ImportAnimalsFromCSV(List<AnimalCSVInfoDTO> animals, Guid org_id);
    }
}

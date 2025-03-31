using CAT.Controllers.DTO;
using CAT.EF.DAL;
using CAT.Models;

namespace CAT.Services.Interfaces
{
    public interface IAnimalService
    {
        void RegistrationAnimal(AnimalRegistrationDTO animal, string? photoUrl);
        void RegistrationNetel(NetelRegistrationDTO animal, string? photoUrl);
        IEnumerable<GroupInfoDTO>? GetGroupsInfo(Guid org_id);
        IEnumerable<IdentificationInfoDTO>? GetIdentificationsFields(Guid org_id);
        public ImportAnimalsInfo ImportAnimalsFromCSV(List<AnimalCSVInfoDTO> animals, Guid org_id);
    }
}

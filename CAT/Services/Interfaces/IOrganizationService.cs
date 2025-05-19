using CAT.Controllers.DTO;
using CAT.EF.DAL;
using CAT.Models;

namespace CAT.Services.Interfaces
{
    public interface IOrganizationService
    {
        bool CheckAnimalById(Guid orgId, Guid? animalId);
        bool CheckGroupById(Guid orgId, Guid? groupId);
        bool CheckDailyActionById(Guid orgId, Guid? actionId);
        bool CheckResearchById(Guid orgId, Guid? researchId);
    }
}
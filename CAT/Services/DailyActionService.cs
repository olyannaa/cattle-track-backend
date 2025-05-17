using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Logic;
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

        public IEnumerable<dynamic> GetDailyActions(Guid organizationId, string type, DailyActionsSortInfoDTO sort)
        {
            return _db.GetDailyActions(organizationId, type, sort);
        }

        public IEnumerable<dynamic>? GetDailyActionsByPage(Guid organizationId, string type,
            DailyActionsSortInfoDTO sort, int page = 1, bool isMoblile = default)
        {
            var (skip, take) = ControllersLogic.ComputePagination(isMoblile, page);
            return _db.GetDailyActionsWithPagination(organizationId, type, sort, skip, take);
        }

        public void DeleteDailyAction(Guid dailyActionId)
        {
            var result = _db.DeleteDailyAction(dailyActionId);

            if (result == 0)
                throw new Exception(message: $"Ошибка. Не удалось удалить ежедневное действие, т.к его не существует.");
        }
        
        public void DeleteResearch(Guid researchId)
        {
            var result = _db.DeleteResearch(researchId);

            if (result == 0)
                throw new Exception(message: $"Ошибка. Не удалось удалить ежедневное действие, т.к его не существует.");
        }

        public void CreateDailyAction(Guid organizationId, CreateDailyActionDTO dto, Guid animalId)
        {
            var guid = Guid.NewGuid();
            if (dto.Type == "Исследования")
                _db.InsertResearch(guid, organizationId, animalId, dto.ResearchName, dto.MaterialType, dto.Date, dto.PerformedBy, dto.Result, dto.Notes);
            else
            {
                _db.InsertDailyAction(guid, animalId, dto.Type, dto.Subtype, dto.Date,
                                    dto.PerformedBy, dto.Result, dto.Medicine, dto.Dose,
                                    dto.Notes, dto.NextDate, dto.OldGroupId, dto.NewGroupId);
            }

            if (dto.Type == "Присвоение номеров")
                _db.UpdateAnimal(animalId, identificationFieldName: dto.Subtype,
                    identificationValue: dto.IdentificationValue);

            if (dto.Type == "Перевод")
                _db.UpdateAnimal(animalId, groupId: dto.NewGroupId);

            if (dto.Type == "Выбытие")
                _db.UpdateAnimal(animalId, status: "Выбывшее", reasonOfDisposal: dto.Subtype);
        }
    }
}
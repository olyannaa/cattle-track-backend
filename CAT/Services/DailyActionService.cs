using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
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

        public IEnumerable<dynamic> GetDailyActions(Guid organizationId, string type)
        {
            return _db.GetDailyActions(organizationId, type);
        }

        public IEnumerable<dynamic>? GetDailyActionsByPage(Guid organizationId, string type, int page = 1, bool isMoblile = default)
        {
            var (skip, take) = ComputePagination(isMoblile, page);
            return _db.GetDailyActionsWithPagination(organizationId, type, skip, take);
        }

        public void DeleteDailyAction(Guid dailyActionId)
        {
            var dailyAction = _db.DailyActions.SingleOrDefault(e => e.Id == dailyActionId);

            if (dailyAction == null)
            {
                var research = _db.Researches.SingleOrDefault(e => e.Id == dailyActionId);

                if (research == null)
                    throw new Exception(message: $"Ошибка. Не удалось удалить ежедневное действие, т.к его не существует.");

                _db.Researches.Remove(research);
                _db.SaveChanges();
                return;
            }

            _db.DailyActions.Remove(dailyAction);
            _db.SaveChanges();
        }

        public void CreateDailyAction(Guid organizationId, CreateDailyActionDTO dto)
        {
            var guid = Guid.NewGuid();
            if (dto.Type == "Исследования")
                _db.InsertResearch(guid, organizationId, dto.AnimalId, dto.ResearchName, dto.MaterialType, dto.Date, dto.PerformedBy, dto.Result, dto.Notes);
            else
            {
                _db.InsertDailyAction(guid, dto.AnimalId, dto.Type, dto.Subtype, dto.Date,
                                    dto.PerformedBy, dto.Result, dto.Medicine, dto.Dose,
                                    dto.Notes, dto.NextDate, dto.OldGroupId, dto.NewGroupId);
            }
            
            if (dto.Type == "Присвоение номеров")
                _db.UpdateAnimal(dto.AnimalId, identificationFieldName: dto.Subtype,
                    identificationValue: dto.IdentificationValue);

            if (dto.Type == "Перевод")
                _db.UpdateAnimal(dto.AnimalId, groupId: dto.NewGroupId);

            if (dto.Type == "Выбытие")
                _db.UpdateAnimal(dto.AnimalId, status: "Выбывшее", reasonOfDisposal: dto.Subtype);
        }

        private static (int skip, int take) ComputePagination(bool isMobile, int page)
        {
            var take = isMobile ? 5 : 10;
            var skip = (page - 1) * take;
            return (skip, take);
        }
    }
}
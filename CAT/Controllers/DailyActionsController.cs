using System.Xml;
using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Logic;
using CAT.Services;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace CAT.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class DailyActionsController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IAuthService _authService;
        private readonly PostgresContext _db;
        private readonly IDailyActionService _daService;

        private readonly IOrganizationService _orgService;

        public DailyActionsController(IAnimalService animalService,
            IAuthService authService, PostgresContext postgresContext,
            IDailyActionService daService, IOrganizationService orgService)
        {
            _animalService = animalService;
            _authService = authService;
            _db = postgresContext;
            _daService = daService;
            _orgService = orgService;
        }
        
        /// <summary>
        /// Информация о списке ежедневных действий для пагинации
        /// </summary>
        /// <param name="type">Тип ежедневного действия</param>
        /// <param name="organizationId">Id организации</param>
        /// <returns></returns>
        [HttpGet, Route("pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetPagination([FromQuery] string type, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var count = _db.GetDailyActions(organizationId, type)?.Count();
            return Ok(new PaginationDTO{Count = count ?? default, EntriesPerPage = entries});
        }
        
        /// <summary>
        /// Возвращение списка ежедневных действий по типу с пагинацией
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetDailyActions([FromHeader] Guid organizationId, [FromQuery] DailyActionsDTO dto)
        {
            var isMobileDevice = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent);
            var dailyActions = _daService.GetDailyActionsByPage(organizationId, dto.Type, dto.SortInfo, dto.Page, isMobileDevice)?.ToList();
            return Ok(dailyActions);
        }

        /// <summary>
        /// Возвращает список животных для ЕД, используя фильтры
        /// </summary>
        /// <param name="organizationId">Id организации</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route("animals")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetAnimalsForDA([FromHeader] Guid organizationId, [FromQuery] DailyAnimalsDTO dto)
        {
            var isMobile = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent);
            return Ok(_animalService.GetAnimalsForDA(organizationId, dto, dto.Page, isMobile));
        }

        /// <summary>
        /// Информация о списке животных для пагинации
        /// </summary>
        /// <param name="type">Тип животного</param>
        /// <param name="organizationId">Id организации</param>
        /// <returns></returns>
        [HttpGet, Route("animals/pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetPagination([FromQuery] DailyAnimalsFilterDTO dto, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var count = _animalService.GetAnimalsForDA(organizationId, new DailyAnimalsDTO { Filter = dto }).Count();
            return Ok(new PaginationDTO{Count = count, EntriesPerPage = entries});
        }

        /// <summary>
        /// Удаляет ежедневные действия
        /// </summary>
        /// <param name="organizationId">Id организации</param>
        /// <param name="actionIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [OrgValidationTypeFilter(checkOrg: true, checkAdmin: true)]
        public IActionResult DeleteDailyAction([FromHeader] Guid organizationId, [FromBody] Guid[] actionIds)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach(var actionId in actionIds)
                {
                    if (!_orgService.CheckDailyActionById(organizationId, actionId))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Одно из ежедневных действий не принадлежит организации пользователя"));
                    }
                    try
                    {
                        _daService.DeleteDailyAction(actionId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO(ex.Message));
                    }
                }
                transaction.Commit();
            }
            return Ok();
        }

        /// <summary>
        /// Удаляет ежедневные действия (исследования)
        /// </summary>
        /// <param name="organizationId">Id организации</param>
        /// <param name="researchIds"></param>
        /// <returns></returns>
        [HttpDelete, Route("researches")]
        [OrgValidationTypeFilter(checkOrg: true, checkAdmin: true)]
        public IActionResult DeleteResearch([FromHeader] Guid organizationId, [FromBody] Guid[] researchIds)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach(var researchId in researchIds)
                {
                    if (!_orgService.CheckResearchById(organizationId, researchId))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Одно из ежедневных действий не принадлежит организации пользователя"));
                    }
                    try
                    {
                        _daService.DeleteResearch(researchId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO(ex.Message));
                    }
                }
                transaction.Commit();
            }
            return Ok();
        }

        /// <summary>
        /// Создаёт ежедневные действия
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="dtoArray"></param>
        /// <returns></returns>
        [HttpPost]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult CreateDailyAction([FromHeader] Guid organizationId, [FromBody] CreateDailyActionDTO[] dtoArray)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach(var dto in dtoArray)
                {
                    if (!_orgService.CheckAnimalById(organizationId, dto.AnimalId))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Один из животных не принадлежит организации пользователя."));
                    }
                    if (dto.NewGroupId != null && !_orgService.CheckGroupById(organizationId, dto.NewGroupId))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Для одного из животных указана группа, не пренадлежащая организации пользователя."));
                    }
                    if (dto.OldGroupId != null && !_orgService.CheckGroupById(organizationId, dto.OldGroupId))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Для одного из животных указана группа, не пренадлежащая организации пользователя."));
                    }
                    try
                    {
                        _daService.CreateDailyAction(organizationId, dto);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO(ex.Message));
                    }
                }
                transaction.Commit();
            }
            return Ok();
        }
    }
}

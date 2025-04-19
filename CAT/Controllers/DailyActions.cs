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

        public DailyActionsController(IAnimalService animalService,
            IAuthService authService, PostgresContext postgresContext,
            IDailyActionService daService)
        {
            _animalService = animalService;
            _authService = authService;
            _db = postgresContext;
            _daService = daService;
        }
        
        [HttpGet, Route("pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetPagination([FromQuery] string type, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var count = _db.GetDailyActions(organizationId, type)?.Count();
            return Ok(new PaginationDTO{Count = count ?? default, EntriesPerPage = entries});
        }
        
        [HttpGet]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetDailyActions([FromHeader] Guid organizationId, [FromQuery] GetDailyActionsDTO dto)
        {
            var isMobileDevice = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent);
            var dailyActions = _daService.GetDailyActionsByPage(organizationId, dto.Type, dto.Page, isMobileDevice)?.ToList();
            return Ok(dailyActions);
        }

        [HttpPost, Route("animals")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetActiveAnimals([FromHeader] Guid organizationId, [FromBody] GetDailyAnimalsDTO dto)
        {
            return Ok(_animalService.GetActiveAnimalsWithFilter(organizationId, dto));
        }
    }
}

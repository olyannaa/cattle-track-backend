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

        public DailyActionsController(IAnimalService animalService,
            IAuthService authService, PostgresContext postgresContext)
        {
            _animalService = animalService;
            _authService = authService;
            _db = postgresContext;
        }
        
        [HttpGet, Route("pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetPagination([FromQuery] string type, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var count = _db.GetDailyActions(organizationId, type).Count();
            return Ok(new PaginationDTO{Count = count, EntriesPerPage = entries});
        }
        
        [HttpGet]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetDailyActions([FromHeader] Guid organizationId, [FromQuery] GetDailyActionsDTO dto)
        {
            return Ok(_db.GetDailyActions(organizationId, dto.Type)?.ToList());
        }

        [HttpPost, Route("animals")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetDailyAnimals([FromHeader] Guid organizationId, [FromBody] GetDailyAnimalsDTO dto)
        {
            return Ok(_db.GetActiveAnimalsWithFilter(organizationId, dto));
        }
    }
}

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
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IAuthService _authService;
        private readonly PostgresContext _db;

        public AnimalsController(IAnimalService animalService,
            IAuthService authService, PostgresContext postgresContext)
        {
            _animalService = animalService;
            _authService = authService;
            _db = postgresContext;
        }

        /// <summary>
        /// Возвращение списка животных по типу с пагинацией
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неверно введённые данные</response>
        /// <response code="401">Не авторизован</response>
        [HttpGet]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetListOfCattle([FromQuery] CensusQueryDTO dto, [FromHeader] Guid organizationId)
        { 
            var isMobileDevice = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent);
            var census = _animalService.GetAnimalCensusByPage(organizationId, dto.Type, dto.Page, isMobileDevice)
                .ToList();
            return Ok(census);
        }
        /// <summary>
        /// Информация о списке животных для пагинации
        /// </summary>
        /// <param name="type">Тип животного</param>
        /// <param name="organizationId">Id организации</param>
        /// <returns></returns>
        [HttpGet, Route("pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetPagination([FromQuery] string type, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var count = _animalService.GetAnimalCensus(organizationId, type).Count();
            return Ok(new PaginationDTO{AnimalCount = count, EntriesPerPage = entries});
        }

        /// <summary>
        /// Обновление данных о животном
        /// </summary>
        /// <param name="id">Id животного</param>
        /// <param name="dto">Редактируемые данные</param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="403">Пользователь не админ или не имеет доступа к организации</response>
        [HttpPut]
        [OrgValidationTypeFilter(checkAdmin: true, checkOrg: true)]
        public IActionResult EditCattleEntry([FromBody] UpdateAnimalDTO[] dtoArray, [FromHeader] Guid organizationId)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach(var dto in dtoArray)
                {
                    if (_db.Animals.Where(x => x.Id == dto.Id).SingleOrDefault()?.OrganizationId != organizationId)
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Один из животных не приналежит организации пользователя"));
                    }

                    if (_db.Groups.Where(x => x.Id == dto.GroupID).SingleOrDefault()?.OrganizationId != organizationId)
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Одиного из животных не возможно добавить в группу, не пренадлежащую организации пользователя"));
                    }
                        
                    var x = _db.UpdateAnimal(dto.Id, dto.TagNumber, null, dto.GroupID, dto.BirthDate, dto.Status);
                }
                transaction.Commit();
            }
            return Ok();
        }

        /// <summary>
        /// Получить группы животных
        /// </summary>
        /// <param name="organizationId">Id организации</param>
        /// <returns></returns>
        [HttpGet, Route("groups")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetAnimalGroups([FromHeader] Guid organizationId)
        {   
            return Ok(_db.Groups.Where(x => x.OrganizationId == organizationId).Select(x => new {x.Name, x.Id}));
        }

        [HttpGet, Route("active")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetActiveCattle([FromHeader] Guid organizationId, [FromQuery] bool pagination)
        { 
            var isMobileDevice = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent);
            var animals = _animalService.GetActiveAnimals(organizationId).ToList();
            return Ok(animals);
        }

        [HttpGet, Route("active/pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetActivePagination([FromQuery] string type, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var count = _animalService.GetActiveAnimals(organizationId).Count();
            return Ok(new PaginationDTO{AnimalCount = count, EntriesPerPage = entries});
        }
    }
}

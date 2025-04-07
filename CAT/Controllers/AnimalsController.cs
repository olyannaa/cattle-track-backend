using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Services;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CAT.Controllers
{
    [Route("api/[controller]")]
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
        /// Возвращение списка животных по типу
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неверно введённые данные</response>
        /// <response code="401">Не авторизован</response>
        [HttpGet, Authorize]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetListOfCattle([FromQuery] CensusQueryDTO dto, [FromHeader] Guid organizationId)
        { 
            var census = _animalService.GetAnimalCensus(organizationId, dto.Type).ToList();
            return Ok(census);
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
        [HttpPut, Authorize]
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
        [HttpGet, Route("groups"), Authorize]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetAnimalGroups([FromHeader] Guid organizationId)
        {   
            return Ok(_db.Groups.Where(x => x.OrganizationId == organizationId).Select(x => new {x.Name, x.Id}));
        }
    }
}

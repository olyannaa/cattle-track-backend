using CAT.Controllers.DTO;
using CAT.EF;
using CAT.Services;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetListOfCattle([FromQuery] CensusQueryDTO dto)
        { 
            if (_db.Organizations.FirstOrDefault(x => x.Id == dto.Id) is null)
                return BadRequest("Организация не найдена");

            var userOrg = _authService.GetUserClaims().Find(x => x.Type == "Organization")?.Value;
            if (Guid.Parse(userOrg) != dto.Id)
                return Forbid();

            var census = _animalService.GetAnimalCensus(dto.Id, dto.Type).ToList();
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
        [HttpPut, Route("{id}"), Authorize]
        public IActionResult EditCattleEntry([FromRoute] Guid id, [FromBody] UpdateAnimalDTO dto)
        {
            var x = _db.UpdateAnimal(id, dto.TagNumber, dto.Type, dto.GroupID, dto.BirthDate, dto.Status);
            return Ok();
        }

        [HttpGet, Route("groups"), Authorize]
        public IActionResult GetAnimalGroups([FromHeader] Guid organizationId)
        { 
            var userOrg = _authService.GetUserClaims().Find(x => x.Type == "Organization")?.Value;
            if (Guid.Parse(userOrg) != organizationId)
                return Forbid();
            
            return Ok(_db.Groups.Where(x => x.OrganizationId == organizationId));
        }
    }
}

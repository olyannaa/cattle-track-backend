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

        [HttpGet, Route("organization/{id}"), Authorize]
        public IActionResult GetListOfCattle([FromRoute] Guid id, [FromQuery] string type)
        { 
            if (_db.Organizations.FirstOrDefault(x => x.Id == id) is null)
                return BadRequest("Организация не найдена");

            var userOrg = _authService.GetUserClaims().Find(x => x.Type == "Organization")?.Value;
            if (Guid.Parse(userOrg) != id)
                return Forbid();

            var census = _animalService.GetAnimalCensus(id, type).ToList();
            return Ok(census);
        }

        [HttpPut, Route("{id}"), Authorize]
        public IActionResult EditCattleEntry([FromRoute] Guid id, [FromBody] UpdateAnimalDTO dto)
        {
            var x = _db.UpdateAnimal(id, dto.TagNumber, dto.Type, dto.GroupID, dto.BirthDate, dto.Status);
            return Ok();
        }
    }
}

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
    public class FilesController : ControllerBase
    {
        private readonly ICSVService _csv;
        private readonly IAnimalService _animalService;
        private readonly IAuthService _authService;
        private readonly PostgresContext _db;

        public FilesController(ICSVService csv, IAnimalService animalService,
            IAuthService authService, PostgresContext postgresContext)
        { 
            _csv = csv;
            _animalService = animalService;
            _authService = authService;
            _db = postgresContext;
        }

        [HttpGet, Route("csv/animals/{id}"), Authorize]
        public IActionResult GetListOfCattle([FromRoute] Guid id, [FromQuery] string type)
        {
            if (_db.Organizations.FirstOrDefault(x => x.Id == id) is null)
                return BadRequest("Организация не найдена");

            var userOrg = _authService.GetUserClaims().Find(x => x.Type == "Organization")?.Value;
            if (Guid.Parse(userOrg) != id)
                return Forbid();

            var census = _animalService.GetAnimalCensus(id, type).ToList();
            var csvFile = _csv.WriteCSV(census);

            return File(csvFile, "application/octet-stream", $"{type}.csv");
        }
    }
}

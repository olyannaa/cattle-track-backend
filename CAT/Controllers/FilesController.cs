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

        /// <summary>
        /// Экспорт списка животных в csv
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("csv/animals"), Authorize]
        public IActionResult GetListOfCattle([FromQuery] CensusQueryDTO dto)
        {
            if (_db.Organizations.FirstOrDefault(x => x.Id == dto.Id) is null)
                return BadRequest("Организация не найдена");

            var userOrg = _authService.GetUserClaims().Find(x => x.Type == "Organization")?.Value;
            if (Guid.Parse(userOrg) != dto.Id)
                return Forbid();

            var census = _animalService.GetAnimalCensus(dto.Id, dto.Type).ToList();
            var csvFile = _csv.WriteCSV(census);

            return File(csvFile, "application/octet-stream", $"{dto.Type}.csv");
        }
    }
}

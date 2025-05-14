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
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetListOfCattle([FromQuery] CensusCsvDTO dto, [FromHeader] Guid organizationId)
        {
            var census = _animalService.GetAnimalCensus(organizationId, dto.Type, dto.SortInfo)
                                        .Select(e => new{ e.TagNumber, e.BirthDate, e.Breed, e.GroupName,
                                            e.Status, e.Origin, e.OriginLocation, e.MotherTagNumber, e.FatherTagNumber })
                                        .ToList();
            var csvFile = _csv.WriteCSV(census);

            return File(csvFile, "application/octet-stream", _csv.GetFileName(dto.Type));
        }
    }
}

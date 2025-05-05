using CAT.EF;
using CAT.Services.Interfaces;
using CAT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAT.Controllers.DTO;

namespace CAT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReproductiveController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IOrganizationService _orgService;

        public ReproductiveController(IAnimalService animalService,IOrganizationService orgService)
        {
            _animalService = animalService;
            _orgService = orgService;
        }

        
        [HttpGet, Route("cow")]
        //[OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> GetAllCows([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetCows(organizationId));
        }

        [HttpGet, Route("bull")]
        //[OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> GetAllBulls([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetBulls(organizationId));
        }

        [HttpPost, Route("insemination")]
        public async Task<IActionResult> InsertInsemination([FromBody] InseminationDTO dto)
        {
            _animalService.InsertInsemination(dto);
            var pregnancy = new PregnancyDTO
            {
                CowId = dto.CowId,
                Date = dto.Date,
                Status = dto.Status,
                ExpectedCalvingDate = dto.ExpectedCalvingDate
            };
            _animalService.InsertPregnancy(pregnancy);
            return Ok(new { Message = "Осеменение успешно зарегистрировано!" });
        }

        [HttpGet, Route("pregnancy")]
        //[OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> GetPregnancies([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetPregnancies(organizationId));
        }
    }
}

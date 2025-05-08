using CAT.Controllers.DTO;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAT.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class ReproductiveController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IOrganizationService _orgService;

        public ReproductiveController(IAnimalService animalService, IOrganizationService orgService)
        {
            _animalService = animalService;
            _orgService = orgService;
        }

        /// <summary>
        /// Получение списка всех коров организации
        /// </summary>
        /// <param name="organizationId">Идентификатор организации</param>
        /// <returns>Список коров</returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неверный идентификатор организации</response>
        /// <response code="401">Не авторизован</response>
        [HttpGet, Route("cow")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> GetAllCows([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetCows(organizationId));
        }

        /// <summary>
        /// Получение списка всех быков организации
        /// </summary>
        /// <param name="organizationId">Идентификатор организации</param>
        /// <returns>Список быков</returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неверный идентификатор организации</response>
        /// <response code="401">Не авторизован</response>
        [HttpGet, Route("bull")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> GetAllBulls([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetBulls(organizationId));
        }

        /// <summary>
        /// Регистрация осеменения с автоматическим созданием записи о беременности
        /// </summary>
        /// <param name="dto">Данные осеменения</param>
        /// <returns>Результат операции</returns>
        /// <response code="200">Осеменение успешно зарегистрировано</response>
        /// <response code="400">Неверные данные осеменения</response>
        /// <response code="401">Не авторизован</response>
        [HttpPost, Route("insemination")]
        public async Task<IActionResult> InsertInsemination([FromBody] InseminationDTO dto)
        {
            _animalService.InsertInsemination(dto);
            var pregnancy = new InsertPregnancyDTO
            {
                CowId = dto.CowId,
                Date = dto.Date,
                Status = "На проверке",
                ExpectedCalvingDate = dto.ExpectedCalvingDate
            };
            _animalService.InsertPregnancy(pregnancy);
            return Ok(new { Message = "Осеменение успешно зарегистрировано!" });
        }

        /// <summary>
        /// Получение списка беременностей организации
        /// </summary>
        /// <param name="organizationId">Идентификатор организации</param>
        /// <returns>Список беременностей</returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неверный идентификатор организации</response>
        /// <response code="401">Не авторизован</response>
        [HttpGet, Route("pregnancy")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> GetPregnancies([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetPregnancies(organizationId));
        }

        /// <summary>
        /// Создание записи о беременности (ручной ввод)
        /// </summary>
        /// <param name="dto">Данные беременности</param>
        /// <returns>Результат операции</returns>
        /// <response code="200">Беременность успешно зарегистрирована</response>
        /// <response code="400">Неверные данные беременности</response>
        /// <response code="401">Не авторизован</response>
        [HttpPost, Route("pregnancy")]
        public async Task<IActionResult> InsertPregnancy([FromBody] InsertPregnancyDTO dto)
        {
            dto.ExpectedCalvingDate = dto.Date.AddDays(285);
            _animalService.InsertPregnancy(dto);
            return Ok(new { Message = "Результат проверки сохранён!" });
        }

        /// <summary>
        /// Регистрация нового отёла
        /// </summary>
        /// <param name="dto">Данные отёла</param>
        /// <returns>Результат операции с информацией о матери и дате</returns>
        /// <response code="200">Отёл успешно зарегистрирован</response>
        /// <response code="400">Неверные данные отёла</response>
        /// <response code="401">Не авторизован</response>
        [HttpPost, Route("calving")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> InsertCalving([FromBody] InsertCalvingDTO dto, [FromHeader] Guid organizationId)
        {
            var id = _animalService.InsertCalving(dto, organizationId);
            return Ok(new { Message = $"✅ Отёл успешно зарегистрирован!🐮 Мать: {dto.CowTagNumber} 📅 Дата отёла: {dto.Date.ToString()}" });
        }
    }
}
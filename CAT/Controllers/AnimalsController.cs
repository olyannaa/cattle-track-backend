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
        private readonly ICSVService _csvService;
        private readonly YandexS3Service _s3Service;
        private readonly IOrganizationService _orgService;

        public AnimalsController(IAnimalService animalService,
            IAuthService authService, PostgresContext postgresContext,
            ICSVService csvService, YandexS3Service s3Service,
            IOrganizationService orgService)
        {
            _animalService = animalService;
            _authService = authService;
            _db = postgresContext;
            _csvService = csvService;
            _s3Service = s3Service;
            _orgService = orgService;
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
            var census = _animalService.GetAnimalCensusByPage(organizationId, dto.Type, dto.SortInfo, dto.Page, isMobileDevice);
            return Ok(census);
        }

        /// <summary>
        /// Возвращение информации о животном
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неверно введённые данные</response>
        /// <response code="401">Не авторизован</response>
        [HttpGet, Route("{animalId}")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetAnimalInfo([FromRoute] Guid animalId, [FromHeader] Guid organizationId)
        {
            if (!_orgService.CheckAnimalById(organizationId, animalId))
                return BadRequest(new ErrorDTO("Запрашиваемое животное не принадлежит организации пользователя"));

            var animal = _animalService.GetAnimalInfo(organizationId, animalId);
            return Ok(animal);
        }
        /// <summary>
        /// Информация о списке животных для пагинации
        /// </summary>
        /// <param name="type">Тип животного</param>
        /// <param name="organizationId">Id организации</param>
        /// <returns></returns>
        [HttpGet, Route("pagination-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetPagination([FromQuery] PaginationQueryDTO dto, [FromHeader] Guid organizationId)
        {
            var entries = ControllersLogic.IsMobileDevice(Request.Headers.UserAgent) ? 5 : 10;
            var sortInfo = new CensusSortInfoDTO { Active = dto.Active ?? default };
            var count = _animalService.GetAnimalCensus(organizationId, dto.Type, sortInfo).Count();
            return Ok(new PaginationDTO { Count = count, EntriesPerPage = entries });
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
                foreach (var dto in dtoArray)
                {
                    if (!_orgService.CheckAnimalById(organizationId, dto.Id))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Один из животных не принадлежит организации пользователя"));
                    }

                    if (dto.GroupID != null && !_orgService.CheckGroupById(organizationId, dto.GroupID))
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO("Одного из животных не возможно добавить в группу, не пренадлежащую организации пользователя"));
                    }
                    try
                    {
                        _animalService.UpdateAnimal(dto);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(new ErrorDTO(ex.Message));
                    }

                }
                transaction.Commit();
            }
            return Ok();
        }

        /// <summary>
        /// Регистрирует новое животное в системе.
        /// </summary>
        /// <param name="body">Данные для регистрации животного, включая фото.</param>
        /// <returns>Сообщение об успешной регистрации и URL загруженного фото.</returns>
        [HttpPost, Route("registration")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public async Task<IActionResult> RegistrationAnimal([FromForm] AnimalRegistrationDTO body, [FromHeader] Guid organizationId)
        {
            var photoUrl = "";
            if (body.Photo != null &&
                new string[] { ".png", ".jpg", ".jpeg" }.Contains(Path.GetExtension(body.Photo.FileName)))
                photoUrl = await _s3Service.UploadFileInS3Async(body.Photo);
            if (body.Type == "Нетель" && (body.InseminationDate == null || body.ExpectedCalvingDate == null
                || body.SpermBatch == null || body.InseminationType == null))
                return BadRequest(new { ErrorText = "Не все обязательные поля заполнены!" });
            _animalService.RegisterAnimal(body, organizationId);
            return Ok(new { Message = "Животное успешно зарегистрировано!" });
        }

        /// <summary>
        /// Импортирует данные о животных из CSV-файла.
        /// </summary>
        [HttpPost, Route("registration/import/csv")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public ActionResult ImportAnimalsFromCSV(IFormFile file, [FromHeader] Guid organizationId)
        {
            if (file == null || !new string[] { ".csv" }.Contains(Path.GetExtension(file.FileName)))
                return BadRequest("Формат файла должен быть .csv");

            var animals = _csvService.ReadAnimalCSV(file.OpenReadStream())
                                     .Select(x =>
                                     {
                                         switch (x.Type)
                                         {
                                             case "1": x.Type = "Бычок"; break;
                                             case "2": x.Type = "Телка"; break;
                                             case "3": x.Type = "Бык"; break;
                                             case "4": x.Type = "Корова"; break;
                                         }
                                         return x;
                                     })
                                     .ToList();

            if (animals.Count == 0) return StatusCode(400);
            var importInfo = _animalService.ImportAnimalsFromCSV(animals, organizationId);
            if (importInfo.Errors > 0) return BadRequest(new { ErrorText = importInfo.Message });
            return Ok(importInfo);
        }

        /// <summary>
        /// Получает информацию о группах животных.
        /// </summary>
        [HttpGet, Route("groups")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetGroups([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetGroupsInfo(organizationId));
        }

        /// <summary>
        /// Получает идентификационные поля для животных.
        /// </summary>
        [HttpGet, Route("identifications")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetIdentificationsFields([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetIdentificationsFields(organizationId));
        }

        /// <summary>
        /// Получает словарь с количеством животных каждого типа организации.
        /// </summary>
        [HttpGet, Route("main-info")]
        [OrgValidationTypeFilter(checkOrg: true)]
        public IActionResult GetMainPageInfo([FromHeader] Guid organizationId)
        {
            return Ok(_animalService.GetMainPageInfo(organizationId));
        }
    }
}

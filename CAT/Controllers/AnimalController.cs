﻿using CAT.Controllers.DTO;
using CAT.EF.DAL;
using CAT.Services;
using CAT.Services.Interfaces;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;

namespace CAT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly ICSVService _csvService;
        private readonly YandexS3Service _s3Service;

        public AnimalController(IAnimalService animalService, ICSVService csvService, YandexS3Service s3Service)
        {
            _animalService = animalService;
            _csvService = csvService;
            _s3Service = s3Service;
        }

        /// <summary>
        /// Регистрирует новое животное в системе.
        /// </summary>
        /// <param name="body">Данные для регистрации животного, включая фото.</param>
        /// <returns>Сообщение об успешной регистрации и URL загруженного фото.</returns>
        [HttpPost, Route("registration")]
        public async Task<IActionResult> RegistrationAnimal([FromForm] AnimalRegistrationDTO body)
        {
            var photoUrl = await UploadFileInS3Async(body.Photo);
            _animalService.RegistrationAnimal(body, photoUrl);
            return Ok(new { Message = "ok", PhotoUrl = photoUrl });
        }

        /// <summary>
        /// Регистрирует нетеля в системе.
        /// </summary>
        [HttpPost, Route("registration/netel")]
        public async Task<IActionResult> RegistrationNetel([FromForm] NetelRegistrationDTO body)
        {
            var photoUrl = await UploadFileInS3Async(body.Photo);
            _animalService.RegistrationNetel(body, photoUrl);
            return Ok(new { Message = "ok", PhotoUrl = photoUrl });
        }

        /// <summary>
        /// Импортирует данные о животных из CSV-файла.
        /// </summary>
        [HttpPost, Route("registration/import/csv")]
        public ActionResult ImportAnimalsFromCSV(IFormFile file, Guid org_id)
        {
            if (file == null || !IsFileExtensionAllowed(file, new string[] { ".csv" }))
                return StatusCode(400);

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
            var importInfo = _animalService.ImportAnimalsFromCSV(animals, org_id);
            return Ok(importInfo);
        }

        /// <summary>
        /// Получает информацию о группах животных.
        /// </summary>
        [HttpGet, Route("groups")]
      
        public IActionResult GetGroups([FromQuery] Guid orgatization_id)
        {
            return Ok(_animalService.GetGroupsInfo(orgatization_id));
        }

        /// <summary>
        /// Получает идентификационные поля для животных.
        /// </summary>
        [HttpGet, Route("identifications")]
       
        public IActionResult GetIdentificationsFields([FromQuery] Guid orgatization_id)
        {
            return Ok(_animalService.GetIdentificationsFields(orgatization_id));
        }

        /// <summary>
        /// Проверяет, является ли загруженный файл CSV-файлом.
        /// </summary>
        public static bool IsFileExtensionAllowed(IFormFile file, string[] allowedExtensions)
        {
            var extension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(extension);
        }

        /// <summary>
        /// Загружает фото животного в S3-хранилище и возвращает его URL.
        /// </summary>
        [SwaggerIgnore]
        public async Task<string> UploadFileInS3Async(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                using var stream = file.OpenReadStream();
                return await _s3Service.UploadFileAsync(stream, fileName, file.ContentType);
            }
            return null;
        }

    }
}
using Amazon.Runtime.Telemetry;
using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Models;
using CAT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System.Globalization;

namespace CAT.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly PostgresContext _db;

        public AnimalService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public List<GroupInfoDTO>? GetGroupsInfo(Guid org_id)
        {
            return _db.GetOrgGroups(org_id)
                .Select(x => new GroupInfoDTO() { Id = x.Id, Name = x.Name})
                .ToList();

        }

        public List<IdentificationInfoDTO>? GetIdentificationsFields(Guid org_id)
        {
            return _db.GetOrgIdentifications(org_id)
                .ToList();
        }

        public void RegisterAnimal(AnimalRegistrationDTO dto, Guid organizationId)
        {
            var animalId = Guid.NewGuid();
            var fatherId = GetAnimalIdByTag(dto.FatherTag);
            var motherId = GetAnimalIdByTag(dto.MotherTag);

            var animal = new Animal
            {
                Id = animalId,
                OrganizationId = organizationId,
                TagNumber = dto.TagNumber,
                BirthDate = dto.BirthDate,
                Type = dto.Type,
                Breed = dto.Breed,
                MotherId = motherId,
                FatherId = fatherId,
                Status = "Активное",
                GroupId = dto.GroupId,
                Origin = dto.Origin,
                OriginLocation = dto.OriginLocation,
            };
            _db.InsertAnimal(animal);
            _db.SaveChanges();

            if (dto.Type == "Нетель")
            {
                try
                {
                    _db.IfNetelInsertReproduction(
                        animalId,
                        dto.InseminationDate,
                        dto.ExpectedCalvingDate,
                        dto.InseminationType,
                        dto.SpermBatch,
                        dto.Technician,
                        dto.Notes);
                }
                catch (PostgresException ex) when (ex.SqlState == "23503")
                {
                    throw new Exception($"Не удалось зарегистрировать осеменение: животное с ID {animalId} не найдено");
                }
            }
            foreach (var animalField in dto.AdditionalFields)
                _db.InsertAnimalIdentification(animalId, animalField.Key, animalField.Value);
        }


        public ImportAnimalsInfo ImportAnimalsFromCSV(List<AnimalCSVInfoDTO> animals, Guid org_id)
        {
            if (animals == null || !animals.Any())
            {
                return new ImportAnimalsInfo
                {
                    Message = "Нет данных для импорта",
                    Errors = 1
                };
            }

            var importInfo = new ImportAnimalsInfo();
            var addedAnimals = new List<(AnimalCSVInfoDTO animal, Guid animalId)>();

            using var transaction = _db.Database.BeginTransaction();

            try
            {
                // Шаг 1 Получение и добавление полей идентификации
                var identificationFields = _db.GetOrgIdentifications(org_id)
                    .Select(x => new IdentificationField { Id = x.Id, FieldName = x.Name })
                    .ToList();

                importInfo.FieldNames = AddNewIdentificationFields(animals, org_id, identificationFields, ref importInfo);
                if (importInfo.Errors > 0)
                {
                    transaction.Rollback();
                    return importInfo;
                }

                // Шаг 2 Подготовка данных животных
                var (activeAnimals, ancestorAnimals) = CategorizeAnimals(animals);
                var sortedAnimals = activeAnimals.Concat(ancestorAnimals).ToList();

                // Шаг 3 Проверка родителей
                var parentTags = sortedAnimals
                    .Where(a => !string.IsNullOrWhiteSpace(a.MotherTag) || !string.IsNullOrWhiteSpace(a.FatherTag))
                    .Select(a => new { a.MotherTag, a.FatherTag })
                    .Distinct()
                    .ToList();

                var existingParents = _db.Animals
                    .Where(a => a.OrganizationId == org_id &&
                               parentTags.Select(p => p.MotherTag).Concat(parentTags.Select(p => p.FatherTag))
                                        .Contains(a.TagNumber))
                    .ToDictionary(a => a.TagNumber, a => a.Id);

                // Шаг 4 Импорт животных
                foreach (var animal in sortedAnimals)
                {
                    if (string.IsNullOrWhiteSpace(animal.TagNumber))
                    {
                        transaction.Rollback();
                        return new ImportAnimalsInfo
                        {
                            Message = $"Ошибка: животное без номера метки не может быть импортировано",
                            Errors = 1,
                            TotalRows = importInfo.TotalRows
                        };
                    }

                    if (!TryParseAnimalData(animal, out var parsedData, out var parseError))
                    {
                        transaction.Rollback();
                        return new ImportAnimalsInfo
                        {
                            Message = $"Ошибка в данных животного {animal.TagNumber}: {parseError}",
                            Errors = 1,
                            TotalRows = importInfo.TotalRows
                        };
                    }

                    var motherId = GetParentId(animal.MotherTag, existingParents);
                    var fatherId = GetParentId(animal.FatherTag, existingParents);

                    try
                    {
                        var originLocation = BuildOriginLocation(animal.OriginFarm, animal.OriginRegion, animal.OriginCountry);
                        var animalId = _db.InsertAnimalToDatabase(org_id, animal, parsedData, motherId, fatherId, originLocation);
                        addedAnimals.Add((animal, animalId));
                        //existingParents.Add(animal.TagNumber, animalId);
                        importInfo.Imported++;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ImportAnimalsInfo
                        {
                            Message = $"Ошибка при импорте животного {animal.TagNumber}: {ex.Message}",
                            Errors = 1,
                            TotalRows = importInfo.TotalRows
                        };
                    }

                    importInfo.TotalRows++;
                }

                // Шаг 5 Добавление полей идентификации
                try
                {
                    AddIdentificationFields(addedAnimals, org_id, ref importInfo);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ImportAnimalsInfo
                    {
                        Message = $"Ошибка при добавлении полей идентификации: {ex.Message}",
                        Errors = 1,
                        TotalRows = importInfo.TotalRows
                    };
                }

                transaction.Commit();
                importInfo.Message = "Импорт успешно завершён.";
                importInfo.Duplicates = animals.Count - sortedAnimals.Count;
                return importInfo;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ImportAnimalsInfo
                {
                    Message = $"Критическая ошибка при импорте: {ex.Message}",
                    Errors = 1,
                    TotalRows = importInfo.TotalRows
                };
            }
        }

        private Guid? GetParentId(string parentTag, Dictionary<string, Guid> existingParents)
        {
            return !string.IsNullOrWhiteSpace(parentTag) && existingParents.TryGetValue(parentTag, out var id)
                ? id
                : null;
        }


        private List<string> AddNewIdentificationFields(List<AnimalCSVInfoDTO> animals, Guid org_id,
            List<IdentificationField> existingFields, ref ImportAnimalsInfo importInfo)
        {
            var createdFields = new List<string>();
            var existingFieldNames = new HashSet<string>(existingFields.Select(f => f.FieldName));

            foreach (var animalField in animals[0].AdditionalFields)
                if (!existingFieldNames.Contains(animalField.Key))
                {
                    try
                    {
                        _db.AddIdentificationField(animalField.Key, org_id);
                        createdFields.Add(animalField.Key);
                        importInfo.CreatedFields++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, $"Ошибка при добавлении поля идентификации {animalField.Key}");
                    }
                }

            return createdFields;
        }

        private (List<AnimalCSVInfoDTO> active, List<AnimalCSVInfoDTO> inactive) CategorizeAnimals(
    List<AnimalCSVInfoDTO> animals)
        {
            var activeAnimals = new List<AnimalCSVInfoDTO>();
            var inactiveAnimals = new List<AnimalCSVInfoDTO>();

            foreach (var animal in animals)
            {

                if (ShouldBeInactive(animal))
                {
                    animal.Status = DetermineInactiveStatus(animal);
                    inactiveAnimals.Add(animal);
                }
                else
                {
                    animal.Status = "Активное";
                    activeAnimals.Add(animal);
                }
            }

            return (activeAnimals, inactiveAnimals);
        }

        private bool ShouldBeInactive(AnimalCSVInfoDTO animal)
        {
            // Критерий 1: Есть признаки выбытия (дата, причина или расход)
            bool hasDisposalInfo = !string.IsNullOrEmpty(animal.DateOfDisposal) ||
                                  !string.IsNullOrEmpty(animal.ReasonOfDisposal) ||
                                  !string.IsNullOrEmpty(animal.Сonsumption);

            // Критерий 2: Статус предка (даже без информации о выбытии)
            bool isAncestor = animal.Status == "Мат.предок" || animal.Status == "Отц.предок";

            return hasDisposalInfo || isAncestor;
        }

        private string DetermineInactiveStatus(AnimalCSVInfoDTO animal)
        {
            // Если есть расход "Продажа" - статус "Проданное"
            if (animal.Сonsumption == "Продажа")
                return "Проданное";

            // Если есть причина выбытия - статус "Выбывшее"
            if (!string.IsNullOrEmpty(animal.ReasonOfDisposal))
                return "Выбывшее";

            // Для предков без конкретной информации
            if (animal.Status == "Мат.предок" || animal.Status == "Отц.предок")
                return "Выбывшее";

            // Дефолтный статус для неактивных животных
            return "Выбывшее";
        }

        private bool TryParseAnimalData(AnimalCSVInfoDTO animal,
                out (DateOnly? birthDate, DateOnly? dateOfReceipt, DateOnly? dateOfDisposal,
                    DateOnly? lastWeightDate, double? lastWeightAtDisposal) parsedData,
                out string error)
        {
            parsedData = default;
            error = null;
            var parseErrors = new List<string>();

            DateOnly? ParseOptionalDate(string dateStr, string fieldName)
            {
                if (string.IsNullOrWhiteSpace(dateStr)) return null;
                var ruRu = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
                if (DateOnly.TryParse(dateStr, ruRu, out var date)) return date;
                parseErrors.Add($"Некорректный формат {fieldName} ('{dateStr}') - будет сохранено как NULL");
                return null;
            }

            var birthDate = ParseOptionalDate(animal.BirthDate, "дата рождения");
            var dateOfReceipt = ParseOptionalDate(animal.DateOfReceipt, "дата поступления");
            var dateOfDisposal = ParseOptionalDate(animal.DateOfDisposal, "дата выбытия");
            var lastWeightDate = ParseOptionalDate(animal.LastWeightDate, "дата взвешивания");

            double? weight = null;
            if (!string.IsNullOrWhiteSpace(animal.LastWeightWeight))
            {
                if (!double.TryParse(animal.LastWeightWeight, out var parsedWeight))
                    parseErrors.Add($"Некорректный вес при выбытии ('{animal.LastWeightWeight}') - будет сохранено как NULL");
                else
                    weight = parsedWeight;
            }

            parsedData = (birthDate, dateOfReceipt, dateOfDisposal, lastWeightDate, weight);

            if (parseErrors.Any())
                error = string.Join("; ", parseErrors);

            return true;
        }

        

        private void AddIdentificationFields(List<(AnimalCSVInfoDTO animal, Guid animalId)> addedAnimals,
            Guid org_id, ref ImportAnimalsInfo importInfo)
        {
            var identificationFields = GetIdentificationsFields(org_id).ToDictionary(f => f.Name, f => f.Id);

            foreach (var (animal, animalId) in addedAnimals)
            {
                foreach (var field in animal.AdditionalFields)
                {
                    if (identificationFields.TryGetValue(field.Key, out var fieldId))
                    {
                        try
                        {
                            _db.InsertAnimalIdentification(animalId, fieldId, field.Value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message, $"Ошибка при добавлении поля идентификации {field.Key} для животного {animal.TagNumber}");
                            importInfo.Errors++;
                        }
                    }
                }
            }
        }

        private string BuildOriginLocation(string farm, string region, string country)
        {
            var parts = new[] { farm, region, country }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts);
        }



        private Guid? GetAnimalIdByTag(string tag)
        {
            var animal = _db.Animals.FirstOrDefault(x => x.TagNumber == tag);
            if (animal == null) return null;
            return animal.Id;
        }
        
        public IEnumerable<AnimalCensus> GetAnimalCensus(Guid organisationId, string animalType, CensusSortInfoDTO sortInfo)
        {
            return _db.GetAnimalsByOrgAndType(organisationId, animalType, sortInfo);
        }

        public IEnumerable<AnimalCensus> GetAnimalCensusByPage(Guid organisationId, string animalType,
            CensusSortInfoDTO sortInfo, int page = 1, bool isMoblile = default)
        {
            var take = isMoblile ? 5 : 10;
            var skip = (page - 1) * take;
            return _db.GetAnimalsWithPagintaion(organisationId, animalType, sortInfo, skip, take);
        }

        public void UpdateAnimal(UpdateAnimalDTO updateInfo)
        {
            _db.UpdateAnimal(updateInfo.Id, updateInfo.TagNumber,
                null, updateInfo.GroupID, updateInfo.BirthDate, updateInfo.Status);
        }

        public IEnumerable<CowDTO> GetCows(Guid organizationId)
            => _db.GetCowsByOrganization(organizationId).ToList();

        public IEnumerable<BullDTO> GetBulls(Guid organizationId)
            => _db.GetBullsByOrganization(organizationId).ToList();

        public void InsertInsemination(InseminationDTO dto)
            => _db.InsertInsemination(dto);

        public void InsertPregnancy(PregnancyDTO dto)
            => _db.InsertPregnancy(dto);

        public void InsertCalving(CalvingDTO dto)
            => _db.InsertCalving(dto);

        public IEnumerable<CowInseminationDTO> GetPregnancies(Guid organizationId)
            => _db.GetPregnancyByOrganization(organizationId).Where(x => x.Status == "Подлежит проверке").ToList();
    }
}

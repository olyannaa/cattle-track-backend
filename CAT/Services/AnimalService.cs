using Amazon.Runtime.Telemetry;
using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Models;
using CAT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            var identificationFields = GetIdentificationsFields(org_id);
            var importInfo = new ImportAnimalsInfo();
            var animalStatuses = new Dictionary<string, string>();
            var existingFields = new HashSet<string>(identificationFields.Select(f => f.Name));
            foreach (var animalField in animals[0].AdditionalFields)
            {
                if (!existingFields.Contains(animalField.Key))
                {
                    _db.AddIdentificationField(animalField.Key, org_id);
                    importInfo.CreatedFields++;
                    importInfo.FieldNames.Add(animalField.Key);
                }
            }
            var allOrgAnimals = _db.Animals.Where(x => x.OrganizationId == org_id).ToList();
            var sortedAnimals = animals
                                                //.Where(x => !allOrgAnimals.Any(a => a.TagNumber == x.TagNumber) && x.TagNumber != "")
                                                .OrderBy(a => a.Status == "Корова стада" ? 0 : 1)
                                                .ToList();
            var activeAnimals = new List<AnimalCSVInfoDTO>();
            var ancestorAnimals = new List<AnimalCSVInfoDTO>();
            foreach (var animal in sortedAnimals)
            {
                if ((animal.Status == "Корова стада" || animal.Status == "Бык стада")
                    || (((animal.Type == "Бычок" || animal.Type == "Телка") && animal.Status != "Мат.предок"
                    && animal.Status != "Отц.предок")))
                    activeAnimals.Add(animal);
                else ancestorAnimals.Add(animal);
            }

            var addedAnimals = new List<AnimalCSVInfoDTO>();
            foreach (var animal in sortedAnimals)
            {
                var originLocation = $"{animal.OriginFarm} {animal.OriginRegion} {animal.OriginCountry}";
                try
                {
                    var birthDate = ParseStringToDate(animal.BirthDate);
                    var dateOfReceipt = ParseStringToDate(animal.DateOfReceipt);
                    var dateOfDisposal = ParseStringToDate(animal.DateOfDisposal);
                    var lastWeightDate = ParseStringToDate(animal.LastWeightDate);
                    var lastWeightAtDisposal = double.Parse(animal.LastWeightWeight);
                    var motherId = _db.Animals
                        .FirstOrDefault(x => x.OrganizationId == org_id && x.TagNumber == animal.MotherTag)?.Id;

                    var fatherId = _db.Animals
                        .FirstOrDefault(x => x.OrganizationId == org_id && x.TagNumber == animal.FatherTag)?.Id;

                    _db.Database.ExecuteSqlRaw("SELECT insert_animal_from_csv({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})",
                        org_id, animal.TagNumber, birthDate, animal.Type,
                        animal.Breed, motherId ?? (object)DBNull.Value, fatherId ?? (object)DBNull.Value, animal.Status,
                        null, "", originLocation, animal.Сonsumption, dateOfReceipt,
                        dateOfDisposal, animal.LastWeightWeight,
                        lastWeightAtDisposal, lastWeightDate, animal.ReasonOfDisposal);

                    importInfo.Imported++;
                    addedAnimals.Add(animal);
                }
                catch (Exception ex)
                {
                    importInfo.Errors++;
                    Console.WriteLine($"Ошибка при вставке животного с номером {animal.TagNumber}: {ex.Message}");
                }


                importInfo.TotalRows++;
            }
            identificationFields = GetIdentificationsFields(org_id);

            foreach (var animal in addedAnimals)
            {
                var animalId = _db.Animals.FirstOrDefault(x => x.OrganizationId == org_id && x.TagNumber == animal.TagNumber).Id;
                foreach (var field in animal.AdditionalFields)
                    _db.InsertAnimalIdentification(animalId, identificationFields.FirstOrDefault(x => x.Name == field.Key).Id, field.Value);
            }
            importInfo.Duplicates = animals.Count() - sortedAnimals.Count();
            return importInfo;
        }


        private Guid? GetAnimalIdByTag(string tag)
        {
            var animal = _db.Animals.FirstOrDefault(x => x.TagNumber == tag);
            if (animal == null) return null;
            return animal.Id;
        }

        static DateOnly? ParseStringToDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            string[] formats =
            {
                "dd.MM.yyyy",
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "dd-MM-yyyy"
            };

            if (DateTime.TryParseExact(dateString.Trim(), formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                return DateOnly.FromDateTime(dateTime);

            return null;
        }
        
        public IEnumerable<AnimalCensus> GetAnimalCensus(Guid organisationId, string animalType, bool isActive = default)
        {
            return _db.GetAnimalsByOrgAndType(organisationId, animalType, isActive);
        }

        public IEnumerable<AnimalCensus> GetAnimalCensusByPage(Guid organisationId, string animalType, bool isActive = default, int page = 1, bool isMoblile = default)
        {
            var take = isMoblile ? 5 : 10;
            var skip = (page - 1) * take;
            return _db.GetAnimalsWithPagintaion(organisationId, animalType, isActive, skip, take);
        }
    }
}

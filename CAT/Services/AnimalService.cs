using Amazon.Runtime.Telemetry;
using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Models;
using CAT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public void RegisterAnimal(AnimalRegistrationDTO dto)
        {
            var animalId = Guid.NewGuid();
            var fatherId = GetAnimalIdByTag(dto.FatherTag);
            var motherId = GetAnimalIdByTag(dto.MotherTag);

            var animal = new Animal
            {
                Id = animalId,
                OrganizationId = dto.OrganizationId,
                TagNumber = dto.TagNumber,
                BirthDate = dto.BirthDate,
                Type = dto.Type,
                Breed = dto.Breed,
                MotherId = motherId,
                FatherId = fatherId,
                Status = dto.Status,
                GroupId = dto.GroupId,
                Origin = dto.Origin,
                OriginLocation = dto.OriginLocation,
            };
            var beforeCount = _db.Animals.Count();
            _db.InsertAnimal(animal);
            _db.SaveChanges();
            var afterCount = _db.Animals.Count();
            Console.WriteLine($"Количество записей в Animals: до = {beforeCount}, после = {afterCount}");

            if (dto.Type == "Нетель")
                _db.IfNetelInsertReproduction( animalId, dto.InseminationDate, dto.ExpectedCalvingDate,
                                    dto.InseminationType, dto.SpermBatch, dto.Technician, dto.Notes);
            foreach (var animalField in dto.AdditionalFields)
                _db.InsertAnimalIdentification(animalId, animalField.Key, animalField.Value);
        }

        public ImportAnimalsInfo ImportAnimalsFromCSV(List<AnimalCSVInfoDTO> animals, Guid org_id)
        {
            var identificationFields = GetIdentificationsFields(org_id);
            var fieldOrder = 1;
            var importInfo = new ImportAnimalsInfo();
            var animalStatuses = new Dictionary<string, string>();
            foreach (var field in identificationFields)
                foreach(var animalField in animals[0].AdditionalFields)
                {
                    if (field.Name != animalField.Key) _db.IdentificationFields.Add(new IdentificationField
                    {
                        Id = Guid.NewGuid(),
                        FieldName = animalField.Key,
                        FieldOrder = fieldOrder++,
                        OrganizationId = org_id
                    });
                    importInfo.CreatedFields++;
                    importInfo.FieldNames.Add(animalField.Key);
                }
            _db.SaveChanges();
            var allOrgAnimals = _db.Animals.Where(x => x.OrganizationId == org_id).ToList();
            var animalsWithoutDuplicates = animals.Where(x => allOrgAnimals.Any(a => a.TagNumber == x.TagNumber) 
                                                              && x.TagNumber != "")
                                                  .OrderBy(a => a.Status == "Корова стада" ? 0 : 1);
            var activeAnimals = new List<AnimalCSVInfoDTO>();
            var ancestorAnimals = new List<AnimalCSVInfoDTO>();
            foreach (var animal in animalsWithoutDuplicates)
            {
                if ((animal.Status == "Корова стада" || animal.Status == "Бык стада") 
                    && ((animal.Type == "Бычок" || animal.Type == "Телка") && animal.Status != "Мат.предок" 
                    && animal.Status != "Отц.предок"))
                    activeAnimals.Add(animal);
                else ancestorAnimals.Add(animal);
            }


            foreach (var animal in animalsWithoutDuplicates)
            {
                var originLocation = $"{animal.OriginFarm} {animal.OriginRegion} {animal.OriginCountry}";
                try
                {
                    var birthDate = ParseStringToDate(animal.BirthDate);
                    var dateOfReceipt = ParseStringToDate(animal.DateOfReceipt);
                    var dateOfDisposal = ParseStringToDate(animal.DateOfDisposal);
                    var lastWeightDate = ParseStringToDate(animal.LastWeightDate);
                    _db.Database.ExecuteSqlRaw("SELECT insert_animal_from_csv({0}, {1}," +
                        "                {2}::date, {3}, {4}, {5}::uuid, {6}::uuid, {7}, {8}::uuid, {9}, {10}," +
                                        "{11},{12},{13},{14},{15},{16},{17})",
                                                        org_id, animal.TagNumber, birthDate, animal.Type,
                                                        animal.Breed, animal.MotherTag ?? null, animal.FatherTag ?? null, animal.Status,
                                                        null, "", originLocation, animal.Сonsumption, dateOfReceipt, dateOfDisposal, animal.LastWeightWeight,
                                                        animal.WeightOfDisposal, lastWeightDate, animal.ReasonOfDisposal);
                    var animalId = _db.Animals.FirstOrDefault(x => x.OrganizationId == org_id && x.TagNumber == animal.TagNumber).Id;
                    foreach (var field in animal.AdditionalFields)
                        _db.InsertAnimalIdentification(animalId, field.Key, field.Value);
                    importInfo.Imported++;
                }
                catch
                {
                    importInfo.Errors++;
                }
                
                
                importInfo.TotalRows++;
            }
            importInfo.Duplicates = animals.Count() - animalsWithoutDuplicates.Count();
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
    }
}

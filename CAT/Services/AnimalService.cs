using Amazon.Runtime.Telemetry;
using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Models;
using CAT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

            if (_db.Groups.Find(dto.GroupId) == null) dto.GroupId = null;
            var animal = new Animal
            {
                Id = animalId,
                OrganizationId = dto.OrganizationId,
                TagNumber = dto.TagNumber,
                BirthDate = dto.BirthDate,
                Breed = dto.Breed,
                MotherId = motherId,
                FatherId = fatherId,
                Status = dto.Status,
                GroupId = dto.GroupId,
                Origin = dto.Origin,
                OriginLocation = dto.OriginLocation,
            };
            _db.InsertAnimal(animal);
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
            foreach(var field in identificationFields)
                foreach(var animalField in animals[0].AdditionalFields)
                {
                    if (field.Name != animalField.Key) _db.IdentificationFields.Add(new IdentificationField
                    {
                        Id = Guid.NewGuid(),
                        FieldName = animalField.Key,
                        FieldOrder = fieldOrder++,
                        OrganizationId = org_id
                    });
                }
            _db.SaveChanges();
            var allOrgAnimals = _db.Animals.Where(x => x.OrganizationId == org_id).ToList();
            var animalsWithoutDuplicates = animals.Where(x => allOrgAnimals.Any(a => a.TagNumber == x.TagNumber));

            foreach (var animal in animalsWithoutDuplicates)
            {
                DateOnly dateOfDisposal, dateOfReception, birthDay, lastWeightDate = new DateOnly(); 
                if (animal.DateOfDisposal != "") dateOfDisposal = DateOnly.Parse(animal.DateOfDisposal);
                if (animal.DateOfReceipt != "") dateOfReception = DateOnly.Parse(animal.DateOfReceipt);
                if (animal.BirthDate != "") birthDay = DateOnly.Parse(animal.BirthDate);
                if (animal.LastWeightDate != "") lastWeightDate = DateOnly.Parse(animal.LastWeightDate);
                var originLocation = $"{animal.OriginFarm} {animal.OriginRegion} {animal.OriginCountry}";
                try
                {
                    _db.Database.ExecuteSqlRaw("SELECT insert_animal_from_csv({0}, {1}," +
                        "                {2}::date, {3}, {4}, {5}::uuid, {6}::uuid, {7}, {8}::uuid, {9}, {10}," +
                                        "{11},{12},{13},{14},{15},{16},{17})",
                                                        org_id, animal.TagNumber, animal.BirthDate, animal.Type,
                                                        animal.Breed, animal.MotherTag ?? null, animal.FatherTag ?? null, animal.Status,
                                                        null, "", originLocation, animal.Сonsumption, animal.DateOfReceipt, animal.DateOfDisposal, animal.LastWeightWeight,
                                                        animal.WeightOfDisposal, animal.LastWeightDate, animal.ReasonOfDisposal);
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
    }
}

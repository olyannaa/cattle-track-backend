using Amazon.S3.Model;
using CAT.Controllers.DTO;
using CAT.EF;
using CAT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CAT.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly PostgresContext _db;

        public OrganizationService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public bool CheckAnimalById(Guid orgId, Guid animalId)
        {
            return _db.Animals.Where(x => x.Id == animalId).SingleOrDefault()?.OrganizationId == orgId;
        }

        public bool CheckGroupById(Guid orgId, Guid groupId)
        {
            return _db.Groups.Where(x => x.Id == groupId).SingleOrDefault()?.OrganizationId == orgId;
        }
    }
}
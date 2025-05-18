using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;
using CAT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CAT.Services
{
    public class GroupService : IGroupService
    {
        private readonly PostgresContext _db;

        public GroupService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public List<GroupTypeDTO> GetGroupTypes(Guid organizationId)
            =>_db.GetGroupTypes(organizationId)
                 .Select(x => new GroupTypeDTO {Id = x.Id, Name = x.Name })
                 .ToList();

        public List<GroupInfrasctructureDTO> GetGroupsByOrganization(Guid organizationId)
            =>_db.Groups
                .Where(g => g.OrganizationId == organizationId)
                .Include(g => g.Type)
                .Select(g => new GroupInfrasctructureDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    TypeId = g.TypeId,
                    TypeName = g.Type.Name,
                    Description = g.Description,
                    Location = g.Location
                }).ToList();

        public List<IdentificationInfoDTO> GetIdentificationsByOrganization(Guid organizationId)
        => _db.GetOrgIdentifications(organizationId)
                 .ToList();

        public bool CreateGroup(CreateGroupDTO dto, Guid organizationId)
        {
            var group = _db.Groups.Where(x => x.OrganizationId == organizationId).FirstOrDefault(x => x.Name == dto.Name);
            if (group != null) return false;
            _db.AddGroup(organizationId, dto.Name, dto.TypeId, dto.Description, dto.Location);
            return true;
        }

        public bool CreateGroupType(CreateGroupTypeDTO dto, Guid organizationId)
        {
            var type = _db.GroupTypes.Where(x => x.OrganizationId == organizationId).FirstOrDefault(x => x.Name == dto.Name);
            if (type != null) return false;
            _db.AddGroupType(organizationId, dto.Name);
            return true;
        }

        public bool CreateIdentification(CreateIdentificationDTO dto, Guid organizationId)
        {
            var identification = _db.IdentificationFields.Where(x => x.OrganizationId == organizationId).FirstOrDefault(x => x.FieldName == dto.Name);
            if (identification != null) return false;
            _db.AddIdentificationField(dto.Name, organizationId);
            return true;
        }

        public bool DeleteGroupType(Guid typeId)
        {
            var countGroups = _db.Groups.Where(x => x.TypeId == typeId).ToList().Count();
            if (countGroups > 0) return false;
            _db.DeleteGroupType(typeId);
            return true;
        }

        public bool DeleteGroup(Guid groupId)
        {
            var countGroups = _db.Animals.Where(x => x.GroupId == groupId).ToList().Count();
            if (countGroups > 0) return false;
            _db.DeleteGroup(groupId);
            return true;
        }

        public void DeleteIdentification(Guid identificationId)
            => _db.DeleteIdentification(identificationId);

        public void EditGroup(EditGroupDTO dto, Guid organizationId)
            => _db.EditGroup(dto.Id, organizationId, dto.Name, dto.TypeId, dto.Description, dto.Location);

        public IEnumerable<string?> GetIdentificationValues(Guid identificationId, Guid orgId, IdentificationValuesFilterDTO? filter)
        {
            return _db.GetIdentificationValues(identificationId, orgId, filter);
        }
    }
}

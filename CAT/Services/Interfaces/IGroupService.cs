using CAT.Controllers.DTO;
using CAT.EF.DAL;

namespace CAT.Services.Interfaces
{
    public interface IGroupService
    {
        public List<GroupTypeDTO> GetGroupTypes(Guid org_id);
        public List<GroupInfrasctructureDTO> GetGroupsByOrganization(Guid org_id);
        public bool CreateGroup(CreateGroupDTO dto, Guid org_id);
        public bool CreateGroupType(CreateGroupTypeDTO dto, Guid org_id);
        public List<IdentificationInfoDTO> GetIdentificationsByOrganization(Guid organizationId);
        public bool CreateIdentification(CreateIdentificationDTO dto, Guid organizationId);
        public bool DeleteGroupType(Guid typeId);
        public bool DeleteGroup(Guid groupId);
        public void DeleteIdentification(Guid groupId);
        void EditGroup(EditGroupDTO dto, Guid organizationId);
    }
}

using CAT.Controllers.DTO;
using CAT.EF;
using Microsoft.EntityFrameworkCore;

namespace CAT.Services
{
    public class UserService : IUserService
    {
        private readonly PostgresContext _db;

        public UserService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public UserInfoDTO? GetUserInfo(string login, string hashedPass)
        {
            var userInfo = _db.GetUserInfo(login, hashedPass)?.Split(", ");

            return userInfo is null ? null : new UserInfoDTO
            {
                Id = userInfo[0],
                OrganizationId = userInfo[1],
                OrganizationName = userInfo[2],
                Name = userInfo[3],
                RoleId = userInfo[4],
                PermissionIds = userInfo[5].Split("; ")
            };
        }
    }
}
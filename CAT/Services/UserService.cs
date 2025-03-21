using CAT.EF;
using DBUserInfo = CAT.EF.DAL.UserInfo;
using UserInfo = CAT.Models.UserInfo;

namespace CAT.Services
{
    public class UserService : IUserService
    {
        private readonly PostgresContext _db;

        public UserService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public UserInfo? GetUserInfo(string login, string hashedPass)
        {
            var dbUserInfo = _db.GetUserInfo(login, hashedPass);
            var dbFirstEntry = dbUserInfo.FirstOrDefault();
            if (dbFirstEntry == null) return null;

            return new UserInfo
            {
                Id = dbFirstEntry.Id,
                RoleId = dbFirstEntry.RoleId,
                OrganizationId = dbFirstEntry.OrganizationId,
                OrganizationName = dbFirstEntry.OrganizationName,
                PermissionIds = dbUserInfo.Select(x => x.PermissionId).ToArray()
            };
        }
    }
}
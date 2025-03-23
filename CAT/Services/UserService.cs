using CAT.Controllers.DTO;
using CAT.EF;

namespace CAT.Services
{
    public class UserService : IUserService
    {
        private readonly PostgresContext _db;

        public UserService(PostgresContext postgresContext)
        {
            _db = postgresContext;
        }

        public UserInfoDTO GetUserInfo(string login, string hashedPass)
        {
            var dbUserInfo = _db.GetUserInfo(login, hashedPass);
            return new UserInfoDTO();
        }
    }
}
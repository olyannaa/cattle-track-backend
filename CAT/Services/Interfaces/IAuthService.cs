
using CAT.Controllers.DTO;

namespace CAT.Services
{
    public interface IAuthService
    {
        UserInfoDTO LogIn(string username, string password);
        void LogOut();
    }
}

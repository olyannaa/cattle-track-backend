
using CAT.Controllers.DTO;
using System.Security.Claims;

namespace CAT.Services
{
    public interface IAuthService
    {
        UserInfoDTO LogIn(string username, string password);
        void LogOut();
        List<Claim> GetUserClaims();
    }
}

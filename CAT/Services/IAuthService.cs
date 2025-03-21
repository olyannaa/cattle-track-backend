
using CAT.Models;

namespace CAT.Services
{
    public interface IAuthService
    {
        UserInfo LogIn(string username, string password);
        void LogOut();
    }
}


using CAT.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CAT.Services
{
    public interface IUserService
    {
        UserInfo? GetUserInfo(string login, string hashedPass);
    }
}

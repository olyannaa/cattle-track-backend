using CAT.Controllers.DTO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CAT.Services.Interfaces
{
    public interface IUserService
    {
        UserInfoDTO? GetUserInfo(string login, string hashedPass);
    }
}

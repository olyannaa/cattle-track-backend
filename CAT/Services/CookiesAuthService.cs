
using CAT.Controllers.DTO;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace CAT.Services
{
    public class CookiesAuthService : IAuthService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public CookiesAuthService(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public UserInfoDTO LogIn(string login, string password)
        {
            var userInfo = _userService.GetUserInfo(login, CalculateSHA256(password));
            if (userInfo is null) throw new NullReferenceException();

            var claimsPrincipal = GetUserPrincipal(userInfo);
            _contextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            ).Wait();
            return userInfo;
        }

        public async void LogOut()
        {
            await _contextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private ClaimsPrincipal GetUserPrincipal(UserInfoDTO userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                new Claim("Organization", userInfo.OrganizationId ?? String.Empty),
                new Claim(ClaimTypes.Role, userInfo.RoleId)
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        }

        private string CalculateSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}

using CAT.Controllers.DTO;
using CAT.EF;
using CAT.Filters;
using CAT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CAT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost, Route("login")]
        [AuthExceptionFilter]
        public ActionResult Login([FromBody] LoginDTO body)
        {
            var userInfo = _authService.LogIn(body.Login, body.Password);
            return Ok(userInfo);
        }

        [HttpPost, Route("logout"), Authorize]
        public ActionResult LogOut()
        {
            _authService.LogOut();
            return Ok();
        }
    }
}

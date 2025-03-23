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

        /// <summary>
        /// Авторизация по логину и паролю на куках
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Неправльно введённые данные</response>
        [HttpPost, Route("login")]
        [AuthExceptionFilter]
        public ActionResult Login([FromBody] LoginDTO body)
        {
            var userInfo = _authService.LogIn(body.Login, body.Password);
            return Ok(userInfo);
        }

        /// <summary>
        /// Отзыв кук с токеном
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Не авторизован</response>
        [HttpPost, Route("logout"), Authorize]
        public ActionResult LogOut()
        {
            _authService.LogOut();
            return Ok();
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class LoginDTO
    {
        /// <example>useruser</example>>
        [Required]
        public string Login { get; set; } = null!;

        /// <example>secure_password</example>>
        [Required]
        public string Password { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace CAT.Controllers.DTO
{
    public class LoginDTO
    {
        [Required]
        public string Login { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}

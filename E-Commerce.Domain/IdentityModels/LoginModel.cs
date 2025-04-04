using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Domain.IdentityModels
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using E_Commerce.Application.Enums;

namespace E_Commerce.Domain.IdentityModels
{
    public class RegisterModel
    {
        [Required, StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required, StringLength(80)]
        public string Username { get; set; }

        [Required, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }

        public UsersRoles Role { get; set; }
    }
}

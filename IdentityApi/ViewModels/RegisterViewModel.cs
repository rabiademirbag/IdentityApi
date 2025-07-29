using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace IdentityApi.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]

        public String Password { get; set; }
    }
}

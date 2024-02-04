using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MySpotify.Models
{
    public class LoginModel
    {
        [Required]
        [DisplayName("Почтовый ящик")]
        public string? Email { get; set; }
        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }
}

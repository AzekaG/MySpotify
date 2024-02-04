using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MySpotify.Models
{
    public class RegistrationModel
    {
        [Required]
        [DisplayName("Имя")]
        public string? FirstName { get; set; }
        [Required]
        [DisplayName("Фамилия")]
        public string? LastName { get; set; }
        [Required]
        [DisplayName("Почтовый ящик")]
        public string? Email { get; set; }

        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DisplayName("Пароль")]
        [Compare("Password", ErrorMessage = "Пароль неверный")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }
    }
}

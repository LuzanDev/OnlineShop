using OnlineShop.Models.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(30,ErrorMessage = "Длина поля не должна превышать 30 символов")]
        [MinLength(2, ErrorMessage = "Длина поля должна быть не менее 2-х символов")]
        [NoSpacesLength(2, 30)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [MaxLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [MaxLength(50,ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [MinLength(5,ErrorMessage = "Длина поля должна быть не менее 5 символов")]
        [NoSpacesLength(5, 50)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}

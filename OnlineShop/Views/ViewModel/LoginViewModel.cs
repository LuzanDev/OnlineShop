using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [MaxLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [MaxLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [MinLength(5, ErrorMessage = "Длина поля должна быть не менее 5 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel.Password
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [MaxLength(50, ErrorMessage = "Длина поля не может превышать 50 символов")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

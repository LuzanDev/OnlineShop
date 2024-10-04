using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel.Password
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
    }

}

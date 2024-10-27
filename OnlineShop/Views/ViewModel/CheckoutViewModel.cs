using OnlineShop.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [MaxLength(30, ErrorMessage = "Длина поля не должна превышать 30 символов")]
        [Display(Name = "Имя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [MaxLength(30, ErrorMessage = "Длина поля не должна превышать 30 символов")]
        [Display(Name = "Фамилия")]
        public string UserSurname { get; set; }

        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [MaxLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [MaxLength(10, ErrorMessage = "Длина поля не должна превышать 10 символов")]
        [Display(Name = "Мобильный телефон")]
        public string UserPhoneNumber { get; set; }
    }
}

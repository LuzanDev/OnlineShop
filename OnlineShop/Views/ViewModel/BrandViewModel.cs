using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class BrandViewModel
    {
        [Required(ErrorMessage = "Название бренда обязательно для заполнения")]
        [MaxLength(50, ErrorMessage = "Максимальная длина поля не должна превышать 50 символов")]
        [MinLength(2, ErrorMessage = "Минимальная длина поля не должна быть меньше 2 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}

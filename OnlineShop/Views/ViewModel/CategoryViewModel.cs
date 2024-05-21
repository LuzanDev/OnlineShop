using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class CategoryViewModel
    {
        [Required(ErrorMessage = "Название категории обязательно для заполнения")]
        [MaxLength(50, ErrorMessage = "Максимальная длина поля не должна превышать 50 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}

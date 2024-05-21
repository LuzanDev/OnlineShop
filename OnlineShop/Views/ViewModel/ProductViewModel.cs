using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Имя товара обязательно для заполнения")]
        [MaxLength(50,ErrorMessage = "Максимальная длина поля не должна превышать 50 символов")]    
        [Display(Name = "Имя товара")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание товара обязательно для заполнения")]
        [MaxLength(100, ErrorMessage = "Максимальная длина поля не должна превышать 100 символов")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Укажите цену товара")]
        [Range(typeof(decimal), "0,01", "9999999,99", ErrorMessage = "Цена должна быть от 0.01 до 9999999.99")]
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите значение из списка.")]
        [Range(1, int.MaxValue, ErrorMessage = "Пожалуйста, выберите значение из списка.")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите значение из списка.")]
        [Range(1, int.MaxValue, ErrorMessage = "Пожалуйста, выберите значение из списка.")]
        public int CategoryId { get; set; }

        [Display(Name = "Изображения")]
        public List<IFormFile> Images { get; set; }
        public string ImageOrder { get; set; }
    }
}

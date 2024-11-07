using OnlineShop.Models.CustomAttribute;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Views.ViewModel
{
    public class CheckoutViewModel : IValidatableObject
    {
        #region OtherValue  
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [MaxLength(30, ErrorMessage = "Длина поля не должна превышать 30 символов")]
        [MinLength(2, ErrorMessage = "Длина поля не должна быть меньше 2-х символов")]
        [RegularExpression("^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Допустимы только буквы.")]
        [Display(Name = "Имя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [MaxLength(30, ErrorMessage = "Длина поля не должна превышать 30 символов")]
        [MinLength(2, ErrorMessage = "Длина поля не должна быть меньше 2-х символов")]
        [RegularExpression("^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Допустимы только буквы.")]
        [Display(Name = "Фамилия")]
        public string UserSurname { get; set; }

        [Required(ErrorMessage = "Введите адрес электронной почты")]
        [MaxLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [MaxLength(13, ErrorMessage = "Длина поля не должна превышать 10 символов")]
        [MinLength(10, ErrorMessage = "Введите корректный номер телефона")]
        [Display(Name = "Мобильный телефон")]
        public string UserPhoneNumber { get; set; }

        [Required(ErrorMessage = "поле обязательно")]
        [Display(Name = "Город")]
        public int CityId { get; set; }
        #endregion


        [Required(ErrorMessage = "Выберите тип доставки")]
        [Display(Name = "Тип доставки")]
        public DeliveryType DeliveryType { get; set; }

        // Поля для самовывоза
        public string NovaPoshtaDepartment { get; set; }

        // Поля для курьерской доставки
        [RequiredIf("DeliveryType", DeliveryType.Courier, ErrorMessage = "Введите улицу для курьерской доставки")]
        [MaxLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        [Display(Name = "Улица")]
        public string Street { get; set; }

        [RequiredIf("DeliveryType", DeliveryType.Courier, ErrorMessage = "Введите номер дома")]
        [MaxLength(10, ErrorMessage = "Длина поля не должна превышать 10 символов")]
        [Display(Name = "Дом")]
        public string HouseNumber { get; set; }

        [MaxLength(10, ErrorMessage = "Длина поля не должна превышать 10 символов")]
        [Display(Name = "Квартира")]
        public string ApartmentNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (DeliveryType == DeliveryType.None)
            {
                yield return new ValidationResult("Выберите тип доставки", new[] { nameof(DeliveryType) });
            }
            if (DeliveryType == DeliveryType.SelfPickup)
            {
                if (string.IsNullOrWhiteSpace(NovaPoshtaDepartment))
                {
                    yield return new ValidationResult("Выберите отделение Новой почты", new[] { nameof(NovaPoshtaDepartment) });
                }

                // Сброс полей курьерской доставки
                Street = null;
                HouseNumber = null;
                ApartmentNumber = null;
            }

            if (DeliveryType == DeliveryType.Courier)
            {
                if (string.IsNullOrWhiteSpace(Street))
                    yield return new ValidationResult("Введите улицу для курьерской доставки", new[] { nameof(Street) });
                if (string.IsNullOrWhiteSpace(HouseNumber))
                    yield return new ValidationResult("Введите номер дома", new[] { nameof(HouseNumber) });

                // Проверка номера квартиры только если он указан
                if (!string.IsNullOrWhiteSpace(ApartmentNumber) && ApartmentNumber.Length > 10)
                {
                    yield return new ValidationResult("Длина поля не должна превышать 10 символов", new[] { nameof(ApartmentNumber) });
                }

                // Сброс значения поля самовывоза
                NovaPoshtaDepartment = null;
            }
        }
    }
}

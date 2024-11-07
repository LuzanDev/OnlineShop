using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models.Enums
{
    public enum DeliveryType
    {
        None,
        [Display(Name = "Самовывоз из Новой почты")]
        SelfPickup,
        [Display(Name = "Доставка курьером")]
        Courier
    }
}

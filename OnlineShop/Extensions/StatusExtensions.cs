using OnlineShop.Models.Enums;

namespace OnlineShop.Extensions
{
    public static class StatusExtensions
    {
        public static string ToDisplayString(this Status status)
        {
            return status switch
            {
                Status.InProcessing => "Формируется",
                Status.Sent => "Отправлен",
                Status.Delivered => "Доставлен",
                _ => "Неизвестный статус"
            };
        }
    }
}

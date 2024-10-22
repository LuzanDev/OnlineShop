using OnlineShop.Models.Entity;

namespace OnlineShop.Views.ViewModel
{
    public class ProductPageViewModel
    {
        public Product Product { get; set; }
        public bool isFavorite { get; set; }
        public bool inCart { get; set; }
    }
}

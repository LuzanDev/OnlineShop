using OnlineShop.Models.Entity;

namespace OnlineShop.Views.ViewModel
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public List<long> FavoriteIds { get; set; }
    }
}

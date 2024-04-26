namespace OnlineShop.Models.Entity
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List <ProductImage> Images { get; set; }
    }
}

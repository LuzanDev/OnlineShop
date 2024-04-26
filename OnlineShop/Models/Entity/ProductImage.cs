namespace OnlineShop.Models.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public Product Product { get; set; }
        public long ProductId { get; set; }
    }
}

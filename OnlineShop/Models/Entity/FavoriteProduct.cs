namespace OnlineShop.Models.Entity
{
    public class FavoriteProduct
    {
        public int Id { get; set; }
        public  string UserId { get; set; }
        public long ProductId { get; set; }
    }
}

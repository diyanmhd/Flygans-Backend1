namespace Flygans_Backend.Dtos.Wishlist
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
    }
}

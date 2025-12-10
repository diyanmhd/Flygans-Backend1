namespace Flygans_Backend.DTOs.Cart;

public class CartItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int Quantity { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flygans_Backend.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    // ✅ LINK TO CART
    [Required]
    public int CartId { get; set; }

    [ForeignKey(nameof(CartId))]
    public Cart Cart { get; set; } = null!;

    // ✅ LINK TO PRODUCT
    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    // ✅ QUANTITY
    [Required]
    public int Quantity { get; set; } = 1;
}

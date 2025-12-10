using Microsoft.EntityFrameworkCore;
using Flygans_Backend.Models;

namespace Flygans_Backend.Data;

public class FlyganDbContext : DbContext
{
    public FlyganDbContext(DbContextOptions<FlyganDbContext> options)
        : base(options)
    {
    }

    // MAIN ENTITIES
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    // ✅ CATEGORY
    public DbSet<Category> Categories { get; set; }

    // ✅ CART ENTITIES
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    // OTHER ENTITIES
    public DbSet<Order> Orders { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
}

using Microsoft.EntityFrameworkCore;
using Flygans_Backend.Models;

namespace Flygans_Backend.Data;

public class FlyganDbContext : DbContext
{
    public FlyganDbContext(DbContextOptions<FlyganDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Order> Orders { get; set; }
}

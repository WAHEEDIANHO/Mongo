using Microsoft.EntityFrameworkCore;
using Mongo.Services.CouponAPI.Model;
using Mongo.Services.ShoppingCartAPI.Models;

namespace Mongo.Services.ShoppingCartAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
            
    }
}

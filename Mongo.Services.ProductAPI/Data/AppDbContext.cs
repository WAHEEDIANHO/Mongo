 using Microsoft.EntityFrameworkCore;
using Mongo.Services.ProductAPI.Model;

namespace Mongo.Services.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }

    } 
}

using Microsoft.EntityFrameworkCore;
using Mongo.Services.OrderAPI.Models;

namespace Mongo.Services.OrderAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrdertDetails> ordertDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

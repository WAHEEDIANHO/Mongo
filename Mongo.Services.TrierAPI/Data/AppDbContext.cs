using Microsoft.EntityFrameworkCore;
using Mongo.Services.TrierAPI.Model;

namespace Mongo.Services.TrierAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Trier> Triers { get; set; }
    }
}

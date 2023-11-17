using Microsoft.EntityFrameworkCore;
using Mongo.Services.RewardAPI.Models;

namespace Mongo.Services.RewardAPI.Data
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Reward> Rewards {  get; set; } 
    }
}

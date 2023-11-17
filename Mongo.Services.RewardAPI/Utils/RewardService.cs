using Microsoft.EntityFrameworkCore;
using Mongo.Services.RewardAPI.Data;
using Mongo.Services.RewardAPI.Models;
using Mongo.Services.RewardAPI.Models.Dto;
using Mongo.Services.RewardAPI.Utils.IUtils;
using System.Text;

namespace Mongo.Services.RewardAPI.Utils
{
    public class RewardService : IRewardService
    {

        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public RewardService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }


        public async Task UpdateReward(RewardsDto message)
        {
            try
            {
                Reward reward = new Reward()
                {
                    OrderId = message.OrderId,
                    RewardActivity = message.RewardActivity,
                    UserId = message.UserId,
                    RewardDate = DateTime.Now
                };

                await using var _db = new AppDbContext(_dbContextOptions);
                await _db.Rewards.AddAsync(reward);
                await _db.SaveChangesAsync();
            }catch (Exception ex) { 
            
            }
        }

    }
}

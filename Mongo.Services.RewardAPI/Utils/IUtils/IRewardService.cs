
using Mongo.Services.RewardAPI.Models.Dto;

namespace Mongo.Services.RewardAPI.Utils.IUtils
{
    public interface IRewardService
    {
        Task UpdateReward(RewardsDto message);
    }
}

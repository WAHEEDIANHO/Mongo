﻿namespace Mongo.Services.EmailAPI.Models.Dto
{
    public class RewardsDto
    {
        public int OrderId {  get; set; }
        public int RewardActivity {  get; set; }
        public string UserId { get; set; }
    }
}

using Azure.Messaging.ServiceBus;
using Mongo.Services.RewardAPI.Models.Dto;
using Mongo.Services.RewardAPI.Utils;
using Newtonsoft.Json;
using System.Text;

namespace Mongo.Services.RewardAPI.Messenger

{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _ServiceConnectionString;
        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedRewardSubscription;
        private readonly RewardService _rewardService;

        private  ServiceBusProcessor  _orderCreatedProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _ServiceConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedTopics");
            _orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedRewardSubcription");

            _rewardService = rewardService;

            var client = new ServiceBusClient(_ServiceConnectionString);

            _orderCreatedProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedRewardSubscription);
        }

        public async Task Start()
        {
            _orderCreatedProcessor.ProcessMessageAsync += MessageRequestReceived;
            _orderCreatedProcessor.ProcessErrorAsync += MessageRequestError;
            await _orderCreatedProcessor.StartProcessingAsync();
            
        }

       
        private async Task MessageRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsDto rewardMsg = JsonConvert.DeserializeObject<RewardsDto>(body);

            try
            {
                await _rewardService.UpdateReward(rewardMsg);
                await args.CompleteMessageAsync(args.Message); 
            }catch (Exception ex)
            {
                throw;
            }
        }

        private Task MessageRequestError(ProcessErrorEventArgs args)
        {
            Console.WriteLine("Eroor getting message");
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
           await _orderCreatedProcessor.StopProcessingAsync();
            await _orderCreatedProcessor.DisposeAsync();
        }
    }

}

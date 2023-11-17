using Azure.Messaging.ServiceBus;
using Mongo.Services.EmailAPI.Model.Dto;
using Mongo.Services.EmailAPI.Models.Dto;
using Mongo.Services.EmailAPI.Utils;
using Newtonsoft.Json;
using System.Text;

namespace Mongo.Services.EmailAPI.Messenger
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _ServiceConnectionString;
        private readonly string _emailCartQueue;
        private readonly string _emailRegUserQueue;
        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedEmailSubscription;
        private readonly EmailService _emailService;

        private  ServiceBusProcessor  _emailProcessor, _emailRegUserProcessor, _orderCreatedEmailProcessor;
        
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _ServiceConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            _emailCartQueue = _configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCartQueue");
            _emailRegUserQueue = _configuration.GetValue<string>("TopicAndQueueName:UserRegisterQueue");
            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedTopics");
            _orderCreatedEmailSubscription = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedEmailSubcription");
           
            _emailService = emailService;

            var client = new ServiceBusClient(_ServiceConnectionString);

            _emailProcessor = client.CreateProcessor(_emailCartQueue);
            _emailRegUserProcessor = client.CreateProcessor(_emailRegUserQueue);
            _orderCreatedEmailProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedEmailSubscription);
        }

        public async Task Start()
        {
            _emailProcessor.ProcessMessageAsync += MessageRequestReceived;
            _emailProcessor.ProcessErrorAsync += MessageRequestError;
            await _emailProcessor.StartProcessingAsync();


            _emailRegUserProcessor.ProcessMessageAsync += RegUserMsgReceived;
            _emailRegUserProcessor.ProcessErrorAsync += MessageRequestError;
            await _emailRegUserProcessor.StartProcessingAsync();

            _orderCreatedEmailProcessor.ProcessMessageAsync += EmailOrderCreated;
            _orderCreatedEmailProcessor.ProcessErrorAsync += MessageRequestError;
            await _orderCreatedEmailProcessor.StartProcessingAsync();
        }

      

        private async Task RegUserMsgReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            UserDto user = JsonConvert.DeserializeObject<UserDto>(body);

            try
            {
                await _emailService.EmailAndLogUser(user);
                await args.CompleteMessageAsync(args.Message);
            }catch
            {
                throw;
            }
        }

        private async Task MessageRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto cart = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                await _emailService.EmailAndLogCart(cart);
                await args.CompleteMessageAsync(args.Message); 
            }catch (Exception ex)
            {
                throw;
            }
        }

        private async Task EmailOrderCreated(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsDto reward = JsonConvert.DeserializeObject<RewardsDto>(body);

            try
            {
                await _emailService.EmailAndLogOrder(reward);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
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
           await _emailProcessor.StopProcessingAsync();
            await _emailProcessor.DisposeAsync();

            await _emailRegUserProcessor.StopProcessingAsync();
            await _emailRegUserProcessor.DisposeAsync();
        }
    }

}

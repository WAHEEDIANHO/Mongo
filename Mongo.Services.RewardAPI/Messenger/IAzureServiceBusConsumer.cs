namespace Mongo.Services.RewardAPI.Messenger
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}

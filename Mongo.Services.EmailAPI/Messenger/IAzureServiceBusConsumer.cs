namespace Mongo.Services.EmailAPI.Messenger
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}

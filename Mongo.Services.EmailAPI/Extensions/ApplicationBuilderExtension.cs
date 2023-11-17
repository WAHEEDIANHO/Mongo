using Mongo.Services.EmailAPI.Messenger;

namespace Mongo.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer serviceBusConsumer {  get; set; }
        public static IApplicationBuilder UseAzureServiceBus(this IApplicationBuilder app)
        {
            serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplication = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplication.ApplicationStarted.Register(OnStart);
            hostApplication.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            serviceBusConsumer.Start();
        }

        private static void OnStop()
        {
            serviceBusConsumer.Stop();
        }
    }
}

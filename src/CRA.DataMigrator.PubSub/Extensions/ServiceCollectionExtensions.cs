using CRA.DataMigrator.PubSub.Configs;
using CRA.DataMigrator.PubSub.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Questrade.Library.PubSubClientHelper.Extensions;
using Questrade.Library.PubSubClientHelper.Primitives;
using Questrade.Library.PubSubClientHelper.Subscriber.Default;
using Questrade.Library.PubSubClientHelper.Subscriber.Default.Configs;
using StatsdClient;

namespace CRA.DataMigrator.PubSub.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSubscriber<TMessage, THandler>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TMessage : class, IMessageWithMetadata, new()
            where THandler : class, IMessageHandler<TMessage>
        {
            var configKey = $"GooglePubSub:Subscribers:{typeof(TMessage).Name}";
            var subscriberConfig = configuration.GetSection(configKey).Get<SubscriberConfig<TMessage>>();
            var ddConfig = configuration.GetSection("DataDog:StatsD").Get<DataDogMetricsConfig>();

            services.RegisterDefaultSubscriber<
                TMessage,
                SubscriberConfig<TMessage>,
                THandler,
                ErrorHandler<TMessage>>(subscriberConfig, ddConfig);

            return services;
        }

        public static IServiceCollection AddPubSub(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            const string errorHandlerConfigKey = "GooglePubSub:Subscribers:ErrorHandler";
            var errorHandlerConfig = configuration.GetSection(errorHandlerConfigKey).Get<ErrorHandlerConfig>();
            services.AddSingleton(errorHandlerConfig);

            return services;
        }

        public static IServiceCollection AddDataDogMetrics(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var dDMetricsConfig = configuration.GetSection(nameof(DataDogMetricsConfig)).Get<DataDogMetricsConfig>();

            services.AddTransient<IDogStatsd>(provider =>
            {
                var dogStatsdService = new DogStatsdService();

                if (dDMetricsConfig is { })
                {
                    var statsdConfig = new StatsdConfig
                    {
                        StatsdServerName = dDMetricsConfig.HostName,
                        Prefix = dDMetricsConfig.Prefix,
                        StatsdPort = dDMetricsConfig.Port,
                    };

                    dogStatsdService.Configure(statsdConfig);
                }

                return dogStatsdService;
            });

            return services;
        }
    }
}
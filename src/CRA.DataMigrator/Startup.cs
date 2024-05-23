using System;
using CRA.DataMigration.Core.Services;
using CRA.DataMigration.DAL;
using CRA.DataMigrator.Handlers;
using CRA.DataMigrator.Managers;
using CRA.DataMigrator.Managers.Abstract;
using CRA.DataMigrator.Models.Messages;
using CRA.DataMigrator.PubSub.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using QT.Clients.CustomerMaster.Ioc;
using Questrade.Library.HealthCheck.AspNetCore.Extensions;

namespace CRA.DataMigrator
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuestradeHealthCheck(_configuration);

            services.TryAddSingleton<ICustomerManager, CustomerManager>();
            services.TryAddSingleton<IMessageManager, MessageManager>();
            services.TryAddSingleton<IMetricService, MetricService>();

            services
                .AddCustomerMasterProvider(_configuration)
                .AddFirestore(_configuration)
                .AddBigQuery(_configuration)
                .AddRepositories();

            services
                .AddDataDogMetrics(_configuration)
                .AddPubSub(_configuration)
                .AddSubscriber<
                    RiskScoreChangedMessage,
                    RiskScoreChangedHandler>(_configuration)
                .AddHealthChecks();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseQuestradeHealthCheck();
        }
    }
}
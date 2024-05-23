using System;
using CRA.DataMigration.Core.Extensions;
using CRA.DataMigration.DAL.Clients.BigQuery;
using CRA.DataMigration.DAL.Clients.Firestore;
using CRA.DataMigration.DAL.Configs;
using CRA.DataMigration.DAL.HealthChecks;
using CRA.DataMigration.DAL.Repositories.BigQuery;
using CRA.DataMigration.DAL.Repositories.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;
using Google.Cloud.Firestore;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CRA.DataMigration.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFirestore(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetConfiguration<FirestoreConfig>();
            if (string.IsNullOrWhiteSpace(config.ProjectId))
            {
                throw new ArgumentNullException(nameof(config.ProjectId));
            }

            services.AddSingleton(serviceProvider =>
            {
                var builder = new FirestoreDbBuilder
                {
                    ProjectId = config.ProjectId
                };

                if (!string.IsNullOrWhiteSpace(config.EmulatorHost))
                {
                    builder.ChannelCredentials = ChannelCredentials.Insecure;
                    builder.Endpoint = config.EmulatorHost;
                }

                return builder.Build();
            });

            services.AddHealthChecks().AddCheck<FirestoreHealthCheck>("firestore-health-check");

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(FirestoreClientFactory<>));
            services.TryAddSingleton<ICustomerRepository, CustomerRepository>();
            services.TryAddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IBigQueryRepository, BigQueryRepository>();

            return services;
        }

        public static IServiceCollection AddBigQuery(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetConfiguration<BigQueryConfig>();

            services.AddSingleton(config);
            services.AddSingleton<ICraBigQueryClient, CraBigQueryClient>();

            return services;
        }
    }
}
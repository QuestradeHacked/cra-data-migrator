using System;
using System.Net.Http.Headers;
using CRA.DataMigration.Core.Extensions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QT.Clients.CustomerMaster.Configs;
using QT.Clients.CustomerMaster.Extensions;
using QT.Clients.CustomerMaster.Providers;

namespace QT.Clients.CustomerMaster.Ioc
{
    public static class IocHandler
    {
        public static IServiceCollection AddCustomerMasterProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICustomerMasterDataProvider, CustomerMasterDataProvider>();
            var config = configuration.GetConfiguration<CustomerMasterDataProviderConfig>();
            services.AddSingleton(config);

            services.AddSingleton<IGraphQLClient>(provider =>
            {
                var environment = provider.GetService<IHostEnvironment>();
                var endpoint = new Uri(config.BaseUrl, "graphql");

                var graphQlClient = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer());

                if (config.Token == null)
                {
                    if (environment.IsProd() || environment.IsUat())
                        throw new Exception("Authorization Token is required to target production.");

                    return graphQlClient;
                }

                graphQlClient.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", config.Token);

                return graphQlClient;
            });

            return services;
        }
    }
}
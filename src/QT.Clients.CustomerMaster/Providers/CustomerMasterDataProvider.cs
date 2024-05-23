using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using QT.Clients.CustomerMaster.Configs;
using QT.Clients.CustomerMaster.Exceptions;
using QT.Clients.CustomerMaster.Models;
using QT.Clients.CustomerMaster.Models.Responses;
using StatsdClient;
using static QT.Clients.CustomerMaster.Models.DataDogTags;

namespace QT.Clients.CustomerMaster.Providers
{
    public class CustomerMasterDataProvider : ICustomerMasterDataProvider
    {
        private readonly IGraphQLClient _graphQlClient;
        private readonly ILogger<CustomerMasterDataProvider> _logger;
        private readonly IDogStatsd _dogStatsd;
        private readonly CustomerMasterDataProviderConfig _customerMasterDataProviderConfig;

        public CustomerMasterDataProvider(
            IGraphQLClient graphQlClient,
            ILogger<CustomerMasterDataProvider> logger,
            IDogStatsd dogStatsd,
            CustomerMasterDataProviderConfig customerMasterDataProviderConfig)
        {
            _graphQlClient = graphQlClient;
            _logger = logger;
            _dogStatsd = dogStatsd;
            _customerMasterDataProviderConfig = customerMasterDataProviderConfig;
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var request = new GraphQLRequest
            {
                OperationName = "User",
                Query = @"
                query User($userId: ID){
                    user(ids: [$userId]){
                        userId
                        personId
                    }
                }",
                Variables = new
                {
                    userId
                }
            };

            var response = await SendQueryWithResilienceAsync<UserDataResponse>(request, cancellationToken);

            if (response.Errors?.Any() == true)
            {
                _dogStatsd.Increment(StatName, tags: new[] {StatusFailed, TypeUser});

                var exceptions = response.Errors.Select(error => new CrmQueryException(error.Message));
                var aggregateException = new AggregateException("GraphQL query error", exceptions);
                _logger.LogError(aggregateException, "GraphQL query error");

                throw new InvalidUserException($"Invalid User Id: {userId}", aggregateException);
            }

            if (response.Data.User?.Count != 1)
            {
                _dogStatsd.Increment(StatName, tags: new[] {StatusFailed, TypeUser});

                _logger.LogError("Invalid number users for this id: {@Result}", response.Data);
                throw new InvalidUserException($"Invalid number users for id: {userId}");
            }

            _dogStatsd.Increment(StatName, tags: new[] {StatusSuccess, TypeUser});
            return response.Data.User.Single();
        }

        public async Task<Person> GetPersonAsync(string personId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(personId))
            {
                throw new ArgumentNullException(nameof(personId));
            }

            var request = new GraphQLRequest
            {
                OperationName = "Person",
                Query = @"
                query Person($personId: ID){
                    person(ids: [$personId]){
                        personId,
                        firstName,
                        lastName
                    }
                }",
                Variables = new
                {
                    personId
                }
            };

            var response = await SendQueryWithResilienceAsync<PersonDataResponse>(request, cancellationToken);

            if (response.Errors?.Any() == true)
            {
                _dogStatsd.Increment(StatName, tags: new[] {StatusFailed, TypePerson});

                var exceptions = response.Errors.Select(error => new CrmQueryException(error.Message));
                var aggregateException = new AggregateException("GraphQL query error", exceptions);
                _logger.LogWarning(aggregateException, "GraphQL query error");

                throw new InvalidPersonException($"Invalid person Id: {personId}", aggregateException);
            }

            _logger.BeginScope("{PersonId}", personId);

            if (response.Data?.Person.Count != 1)
            {
                _dogStatsd.Increment(StatName, tags: new[] {StatusFailed, TypePerson});

                throw new InvalidPersonException(
                    $"Invalid number of persons were returned for a query. Number of users: {response.Data?.Person.Count}");
            }

            _dogStatsd.Increment(StatName, tags: new[] {StatusSuccess, TypePerson});

            return response.Data?.Person.Single();
        }

        private async Task<GraphQLResponse<T>> SendQueryWithResilienceAsync<T>(GraphQLRequest request, CancellationToken cancellationToken)
        {
            var delay = Backoff
                .DecorrelatedJitterBackoffV2(
                    TimeSpan.FromMilliseconds(_customerMasterDataProviderConfig.Resilience.TimeBetweenRetryInMilliseconds),
                    _customerMasterDataProviderConfig.Resilience.RetryCount
                );

            var policy = Policy
                .Handle<Exception>()
                .OrResult<GraphQLResponse<T>>(r => r.Errors?.Any() == true)
                .WaitAndRetryAsync(delay, onRetry: OnPolicyRetry);

            // Execute a function returning a result
            return await policy.ExecuteAsync(context =>
                _graphQlClient.SendQueryAsync<T>(request, cancellationToken), cancellationToken: cancellationToken);
        }

        private void OnPolicyRetry<T>(DelegateResult<GraphQLResponse<T>> outcome, TimeSpan timespan, int retryAttempt, Context context)
        {
            _logger.LogWarning(outcome.Exception, "{Retry} attempt failed, next retry in {Delay} ms", retryAttempt, timespan.TotalMilliseconds);
        }
    }
}
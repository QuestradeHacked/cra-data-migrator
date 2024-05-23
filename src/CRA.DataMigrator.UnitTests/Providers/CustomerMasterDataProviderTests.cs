using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using QT.Clients.CustomerMaster.Configs;
using QT.Clients.CustomerMaster.Exceptions;
using QT.Clients.CustomerMaster.Models;
using QT.Clients.CustomerMaster.Models.Responses;
using QT.Clients.CustomerMaster.Providers;
using StatsdClient;
using Xunit;

namespace CRA.DataMigrator.UnitTests.Providers
{
    public class CustomerMasterDataProviderTests
    {
        private const string Id = "12345";

        private readonly Mock<IGraphQLClient> _graphQlClientMock;
        private readonly Mock<ILogger<CustomerMasterDataProvider>> _loggerMock;
        private readonly Mock<IDogStatsd> _dataDogClient;
        private readonly CustomerMasterDataProviderConfig _customerMasterDataProviderConfig;

        private readonly CustomerMasterDataProvider _customerMasterDataProvider;

        public CustomerMasterDataProviderTests()
        {
            _graphQlClientMock = new Mock<IGraphQLClient>();
            _loggerMock = new Mock<ILogger<CustomerMasterDataProvider>>();
            _dataDogClient = new Mock<IDogStatsd>();
            _customerMasterDataProviderConfig = new CustomerMasterDataProviderConfig
            {
                Resilience = new Resilience
                {
                    RetryCount = 2,
                }
            };

            _customerMasterDataProvider = new CustomerMasterDataProvider(
                _graphQlClientMock.Object,
                _loggerMock.Object,
                _dataDogClient.Object,
                _customerMasterDataProviderConfig);
        }

        [Fact]
        public async Task GetUserAsync_ThrowsException_UserIdIsNull()
        {
            Func<Task> action = () => _customerMasterDataProvider.GetUserAsync(string.Empty);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetUserAsync_ThrowsException_ErrorInResponse()
        {
            var response = new GraphQLResponse<UserDataResponse>
            {
                Errors = new[] {new GraphQLError()}
            };

            _graphQlClientMock.Setup(client =>
                    client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .ReturnsAsync(response);

            Func<Task> action = () => _customerMasterDataProvider.GetUserAsync(Id);

            await action.Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task GetUserAsync_ThrowsException_FoundMoreThanOneUser()
        {
            var response = new GraphQLResponse<UserDataResponse>
            {
                Data = new UserDataResponse
                {
                    User = new List<User>
                    {
                        new User(),
                        new User(),
                    }
                }
            };

            _graphQlClientMock.Setup(client =>
                    client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .ReturnsAsync(response);

            Func<Task> action = () => _customerMasterDataProvider.GetUserAsync(Id);

            await action.Should().ThrowAsync<InvalidUserException>();
        }

        [Fact]
        public async Task GetUserAsync_ReturnsUser_FoundOnlySingleUser()
        {
            var user = new User();

            var response = new GraphQLResponse<UserDataResponse>
            {
                Data = new UserDataResponse
                {
                    User = new List<User>
                    {
                        user
                    }
                }
            };

            _graphQlClientMock.Setup(client =>
                    client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .ReturnsAsync(response);

            var result = await _customerMasterDataProvider.GetUserAsync(Id);

            result.Should().Be(user);
        }

        [Fact]
        public async Task GetPersonAsync_ThrowsException_PersonIdIsNull()
        {
            Func<Task> action = () => _customerMasterDataProvider.GetPersonAsync(string.Empty);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetPersonAsync_ThrowsException_ErrorInResponse()
        {
            var response = new GraphQLResponse<PersonDataResponse>
            {
                Errors = new[] {new GraphQLError()}
            };

            _graphQlClientMock.Setup(client =>
                    client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .ReturnsAsync(response);

            Func<Task> action = () => _customerMasterDataProvider.GetPersonAsync(Id);

            await action.Should().ThrowAsync<InvalidPersonException>();
        }

        [Fact]
        public async Task GetPersonAsync_ThrowsException_FoundMoreThanOneUser()
        {
            var response = new GraphQLResponse<PersonDataResponse>
            {
                Data = new PersonDataResponse
                {
                    Person = new List<Person>
                    {
                        new Person(),
                        new Person(),
                    }
                }
            };

            _graphQlClientMock.Setup(client =>
                    client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .ReturnsAsync(response);

            Func<Task> action = () => _customerMasterDataProvider.GetPersonAsync(Id);

            await action.Should().ThrowAsync<InvalidPersonException>();
        }

        [Fact]
        public async Task GetPersonAsync_ReturnsData_FoundSinglePerson()
        {
            var person = new Person();

            var response = new GraphQLResponse<PersonDataResponse>
            {
                Data = new PersonDataResponse
                {
                    Person = new List<Person>
                    {
                        person
                    }
                }
            };

            _graphQlClientMock.SetupSequence(client =>
                    client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .Throws<Exception>()
                .ReturnsAsync(response);

            var result = await _customerMasterDataProvider.GetPersonAsync(Id);

            result.Should().Be(person);
        }

        [Fact]
        public async Task GetPersonAsync_RetriesHttpCall_ExceptionOccured()
        {
            var response = new GraphQLResponse<PersonDataResponse>
            {
                Data = new PersonDataResponse
                {
                    Person = new List<Person> { new Person() }
                }
            };

            _graphQlClientMock.SetupSequence(client =>
                    client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .Throws<Exception>()
                .ReturnsAsync(response);

            await _customerMasterDataProvider.GetPersonAsync(Id);

            _graphQlClientMock.Verify(
                client => client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetPersonAsync_ThrowsException_ExceededRetriesCount()
        {
            var response = new GraphQLResponse<PersonDataResponse>
            {
                Errors = new[] {new GraphQLError()}
            };

            _graphQlClientMock.SetupSequence(client =>
                    client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .Throws<Exception>()
                .ReturnsAsync(response);

            Func<Task> action = () => _customerMasterDataProvider.GetPersonAsync(Id);

            await action.Should().ThrowAsync<Exception>();

            _graphQlClientMock.Verify(
                client => client.SendQueryAsync<PersonDataResponse>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(_customerMasterDataProviderConfig.Resilience.RetryCount + 1));
        }

        [Fact]
        public async Task GetUserAsync_RetriesHttpCall_ExceptionOccured()
        {
            var response = new GraphQLResponse<UserDataResponse>
            {
                Data = new UserDataResponse
                {
                    User = new List<User> { new User() }
                }
            };

            _graphQlClientMock.SetupSequence(client =>
                    client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .Throws<Exception>()
                .ReturnsAsync(response);

            await _customerMasterDataProvider.GetUserAsync(Id);

            _graphQlClientMock.Verify(
                client => client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetUserAsync_ThrowsException_ExceededRetriesCount()
        {
            var response = new GraphQLResponse<UserDataResponse>
            {
                Errors = new[] {new GraphQLError()}
            };

            _graphQlClientMock.SetupSequence(client =>
                    client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(), CancellationToken.None))
                .Throws<Exception>()
                .Throws<Exception>()
                .ReturnsAsync(response);

            Func<Task> action = () => _customerMasterDataProvider.GetUserAsync(Id);

            await action.Should().ThrowAsync<Exception>();

            _graphQlClientMock.Verify(
                client => client.SendQueryAsync<UserDataResponse>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(_customerMasterDataProviderConfig.Resilience.RetryCount + 1));
        }
    }
}
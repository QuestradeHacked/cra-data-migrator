using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;
using CRA.DataMigrator.Managers;
using FluentAssertions;
using Moq;
using QT.Clients.CustomerMaster.Models;
using QT.Clients.CustomerMaster.Providers;
using Xunit;

namespace CRA.DataMigrator.UnitTests.Managers
{
    public class CustomerManagerTests
    {
        private const string Id = "12345";

        private readonly Mock<ICustomerMasterDataProvider> _customerMasterDataProvider;
        private readonly Mock<ICustomerRepository> _customerRepository;

        private readonly CustomerManager _customerManager;

        public CustomerManagerTests()
        {
            _customerMasterDataProvider = new Mock<ICustomerMasterDataProvider>();
            _customerRepository = new Mock<ICustomerRepository>();

            _customerManager = new CustomerManager(_customerMasterDataProvider.Object, _customerRepository.Object);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsDataFromRepo_WhenFound()
        {
            _customerRepository.Setup(repository => repository.GetByIdAsync(
                    Id, CancellationToken.None))
                .ReturnsAsync(new Customer());

            await _customerManager.FindByIdAsync(Id, CancellationToken.None);

            _customerMasterDataProvider.Verify(
                provider => provider.GetUserAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsDataFromCustomerMaster_WhenNotFoundInRepo()
        {
            var person = new Person
            {
                FirstName = Id,
                LastName = Id
            };

            _customerMasterDataProvider
                .Setup(provider => provider.GetUserAsync(
                    It.IsAny<string>(),
                    CancellationToken.None))
                .ReturnsAsync(new User());
            _customerMasterDataProvider
                .Setup(provider => provider.GetPersonAsync(
                    It.IsAny<string>(),
                    CancellationToken.None))
                .ReturnsAsync(person);

            var customer = await _customerManager.FindByIdAsync(Id, CancellationToken.None);

            customer.FirstName.Should().Be(person.FirstName);
            customer.LastName.Should().Be(person.LastName);
        }

        [Fact]
        public async Task FindByIdAsync_SavesCustomer_WhenRequestedFromCustomerMaster()
        {
            var person = new Person
            {
                FirstName = Id,
                LastName = Id
            };

            _customerMasterDataProvider
                .Setup(provider => provider.GetUserAsync(
                    It.IsAny<string>(),
                    CancellationToken.None))
                .ReturnsAsync(new User());
            _customerMasterDataProvider
                .Setup(provider => provider.GetPersonAsync(
                    It.IsAny<string>(),
                    CancellationToken.None))
                .ReturnsAsync(person);

            var customer = await _customerManager.FindByIdAsync(Id, CancellationToken.None);

            _customerRepository.Verify(repository => repository.UpsertAsync(customer, CancellationToken.None),
                Times.Once);
        }
    }
}
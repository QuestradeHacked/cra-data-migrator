using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;
using CRA.DataMigrator.Managers.Abstract;
using QT.Clients.CustomerMaster.Providers;

namespace CRA.DataMigrator.Managers
{
    public class CustomerManager : ICustomerManager
    {
        private readonly ICustomerMasterDataProvider _customerMasterDataProvider;
        private readonly ICustomerRepository _customerRepository;

        public CustomerManager(
            ICustomerMasterDataProvider customerMasterDataProvider,
            ICustomerRepository customerRepository)
        {
            _customerMasterDataProvider = customerMasterDataProvider;
            _customerRepository = customerRepository;
        }

        public async Task<Customer> FindByIdAsync(string customerId, CancellationToken token)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, token);

            if (customer != null) return customer;

            var user = await _customerMasterDataProvider.GetUserAsync(customerId, token);
            var person = await _customerMasterDataProvider.GetPersonAsync(user.PersonId, token);

            customer = new Customer
            {
                Id = customerId,
                FirstName = person.FirstName,
                LastName = person.LastName,
            };

            await _customerRepository.UpsertAsync(customer, token);

            return customer;
        }
    }
}
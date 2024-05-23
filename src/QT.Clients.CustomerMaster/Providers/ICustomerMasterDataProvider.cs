using System.Threading;
using System.Threading.Tasks;
using QT.Clients.CustomerMaster.Models;

namespace QT.Clients.CustomerMaster.Providers
{
    public interface ICustomerMasterDataProvider
    {
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<Person> GetPersonAsync(string personId, CancellationToken cancellationToken = default);
    }
}
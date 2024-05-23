using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.Firestore;

namespace CRA.DataMigrator.Managers.Abstract
{
    public interface ICustomerManager
    {
        Task<Customer> FindByIdAsync(string customerId, CancellationToken token);
    }
}
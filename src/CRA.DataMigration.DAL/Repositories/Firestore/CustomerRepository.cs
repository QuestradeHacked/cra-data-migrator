using CRA.DataMigration.DAL.Clients.Firestore;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;

namespace CRA.DataMigration.DAL.Repositories.Firestore
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(
            FirestoreClientFactory<Customer> firestoreClientFactory)
            : base(firestoreClientFactory, "Customers")
        {
        }
    }
}
using CRA.DataMigration.DAL.Entities.Firestore;
using Google.Cloud.Firestore;

namespace CRA.DataMigration.DAL.Clients.Firestore
{
    public class FirestoreClientFactory<TEntity> where TEntity : FirestoreBaseEntity
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreClientFactory(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public FirestoreClient<TEntity> Create(string collectionName)
        {
            var client = new FirestoreClient<TEntity>(_firestoreDb, collectionName);

            return client;
        }
    }
}
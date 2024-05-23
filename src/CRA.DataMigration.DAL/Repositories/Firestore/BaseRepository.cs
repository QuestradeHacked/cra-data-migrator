using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Clients.Firestore;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;

namespace CRA.DataMigration.DAL.Repositories.Firestore
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : FirestoreBaseEntity
    {
        protected readonly FirestoreClient<TEntity> FirestoreClient;

        protected BaseRepository(FirestoreClientFactory<TEntity> firestoreClientFactory, string collection)
        {
            FirestoreClient = firestoreClientFactory.Create(collection);
        }

        public Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return FirestoreClient.GetByIdAsync(id, cancellationToken);
        }

        public async Task<TEntity> GetSingleByAsync(
            Expression<Func<TEntity, object>> property,
            object value,
            CancellationToken cancellationToken = default)
        {
            var entities = await FirestoreClient.GetByAsync(property, value, cancellationToken);

            return entities.SingleOrDefault();
        }

        public virtual async Task<string> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var createdEntity = await FirestoreClient.UpsertAsync(entity, cancellationToken);

            return createdEntity.Id;
        }
    }
}
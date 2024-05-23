using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.Firestore;

namespace CRA.DataMigration.DAL.Repositories.Firestore.Abstract
{
    public interface IBaseRepository<TEntity> where TEntity : FirestoreBaseEntity
    {
        Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<TEntity> GetSingleByAsync(
            Expression<Func<TEntity, object>> property,
            object value,
            CancellationToken cancellationToken = default);

        Task<string> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
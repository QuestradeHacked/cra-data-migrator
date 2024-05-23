using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.Firestore;
using Google.Cloud.Firestore;

namespace CRA.DataMigration.DAL.Clients.Firestore
{
    public class FirestoreClient<T> where T : FirestoreBaseEntity
    {
        private readonly FirestoreDb _firestoreDb;

        public readonly CollectionReference WorkingCollection;

        public FirestoreClient(FirestoreDb firestoreDb, string collectionName)
        {
            _firestoreDb = firestoreDb;
            WorkingCollection = firestoreDb?.Collection(collectionName);
        }

        public virtual async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var querySnapshot = await WorkingCollection.Document(id).GetSnapshotAsync(cancellationToken);

            return querySnapshot.Exists ? querySnapshot.ConvertTo<T>() : default;
        }

        public virtual async Task<List<T>> GetByAsync(Expression<Func<T, object>> property, object value,
            CancellationToken cancellationToken = default)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var propertyName = GetCorrectPropertyName(property);

            var querySnapshot = await WorkingCollection.WhereEqualTo(propertyName, value)
                .GetSnapshotAsync(cancellationToken);

            return querySnapshot.Documents.Select(x => x.ConvertTo<T>()).ToList();
        }

        public virtual async Task<T> UpsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            DocumentReference document;

            if (!string.IsNullOrWhiteSpace(entity.Id))
            {
                // Can also create document with a custom Id here.
                document = WorkingCollection.Document(entity.Id);
                await document.SetAsync(entity, SetOptions.Overwrite, cancellationToken);
            }
            else
            {
                // Auto-generates Id
                document = await WorkingCollection.AddAsync(entity, cancellationToken);
            }

            var entitySnapshot = await document.GetSnapshotAsync(cancellationToken);

            return entitySnapshot.ConvertTo<T>();
        }

        public async Task RunTransactionAsync(
            Func<Transaction, Task> callback,
            TransactionOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _firestoreDb.RunTransactionAsync(callback, options, cancellationToken);
        }

        private static string GetCorrectPropertyName(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            var op = ((UnaryExpression) expression.Body).Operand;
            return ((MemberExpression) op).Member.Name;
        }
    }
}
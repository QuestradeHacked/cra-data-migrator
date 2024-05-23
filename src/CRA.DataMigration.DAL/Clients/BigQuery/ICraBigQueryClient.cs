using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.BigQuery.V2;

namespace CRA.DataMigration.DAL.Clients.BigQuery
{
    public interface ICraBigQueryClient
    {
        Task InsertRowAsync(BigQueryInsertRow insertRow, CancellationToken cancellationToken = default);
    }
}
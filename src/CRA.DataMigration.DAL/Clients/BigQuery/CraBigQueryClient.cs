using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Configs;
using Google.Cloud.BigQuery.V2;

namespace CRA.DataMigration.DAL.Clients.BigQuery
{
    public class CraBigQueryClient : ICraBigQueryClient
    {
        private readonly BigQueryClient _client;
        private readonly BigQueryConfig _config;

        public CraBigQueryClient(BigQueryConfig config)
        {
            _config = config;
            _client = BigQueryClient.Create(_config.ProjectId);
        }

        public async Task InsertRowAsync(BigQueryInsertRow insertRow, CancellationToken cancellationToken = default)
        {
            var result = await _client.InsertRowAsync(_config.DatasetName, _config.TableName, insertRow,
                cancellationToken: cancellationToken);

            if (result.Status != BigQueryInsertStatus.AllRowsInserted)
            {
                result.ThrowOnAnyError();
            }
        }
    }
}
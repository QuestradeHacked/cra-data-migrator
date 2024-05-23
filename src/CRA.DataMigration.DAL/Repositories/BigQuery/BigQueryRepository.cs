using System;
using System.Threading.Tasks;
using CRA.DataMigration.Core.Services;
using CRA.DataMigration.DAL.Clients.BigQuery;
using CRA.DataMigration.DAL.Entities.BigQuery;
using CRA.DataMigration.DAL.Helpers;
using Google;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SerilogTimings;

namespace CRA.DataMigration.DAL.Repositories.BigQuery
{
    public class BigQueryRepository : IBigQueryRepository
    {
        private readonly ICraBigQueryClient _client;

        private readonly IMetricService _metricService;
        private readonly ILogger<BigQueryRepository> _logger;

        public BigQueryRepository(ILogger<BigQueryRepository> logger, IMetricService metricService,
            ICraBigQueryClient client)
        {
            _metricService = metricService;
            _client = client;
            _logger = logger;
        }

        public async Task AddRecordAsync(RiskRatingEntity record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            using (Operation.Time("Adding BigQuery row for {CustomerId}", record.CustomerId))
            {
                var insertRow = BigQueryHelper.ConvertToInsertRow(record);

                try
                {
                    await _client.InsertRowAsync(insertRow);
                }
                catch (GoogleApiException ex)
                {
                    _logger.LogError(ex, "Data cannot be inserted to the Database");
                    throw;
                }
            }
            _metricService.Increment("bigQuery.RowInserted");
        }
    }
}
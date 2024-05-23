using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.BigQuery;

namespace CRA.DataMigration.DAL.Repositories.BigQuery
{
    public interface IBigQueryRepository
    {
        Task AddRecordAsync(RiskRatingEntity record);
    }
}
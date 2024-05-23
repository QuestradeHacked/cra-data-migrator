using System.Collections.Generic;

namespace CRA.DataMigration.DAL.Entities.BigQuery
{
    public class RiskRatingEntity : IBigQueryEntity
    {
        public string RiskRatingId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerFullName { get; set; }

        public int TotalPoints { get; set; }

        public string Rating { get; set; }

        public string PreviousRating { get; set; }

        public string RatingTime { get; set; }

        public IList<RiskFactorEntity> Factors { get; set; }

        public string GetId()
        {
            return RiskRatingId;
        }
    }
}
using System;
using System.Collections.Generic;

namespace CRA.DataMigrator.Models.Data
{
    public class RiskScoreChangedData
    {
        public string RiskRatingId { get; set; }

        public string CustomerId { get; set; }

        public int TotalPoints { get; set; }

        public string Rating { get; set; }

        public string PreviousRating { get; set; }

        public DateTime RatingTime { get; set; }

        public ICollection<RiskRatingFactor> Factors { get; set; }
    }
}
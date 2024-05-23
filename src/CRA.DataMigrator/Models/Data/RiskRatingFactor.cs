namespace CRA.DataMigrator.Models.Data
{
    public class RiskRatingFactor
    {
        public string RiskRatingFactorId { get; set; }

        public string FactorName { get; set; }

        public string FactorValue { get; set; }

        public string PreviousFactorValue { get; set; }

        public int Points { get; set; }
    }
}
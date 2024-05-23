namespace CRA.DataMigration.DAL.Entities.BigQuery
{
    public class RiskFactorEntity
    {
        public string FactorName { get; set; }

        public string FactorValue { get; set; }

        public string PreviousFactorValue { get; set; }

        public int Points { get; set; }
    }
}
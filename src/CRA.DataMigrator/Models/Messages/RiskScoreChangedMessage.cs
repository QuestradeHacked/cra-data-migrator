using CRA.DataMigrator.Models.Data;
using Questrade.Library.PubSubClientHelper.Subscriber.Default.Models;

namespace CRA.DataMigrator.Models.Messages
{
    public class RiskScoreChangedMessage : MessageWithMetadata<RiskScoreChangedData>
    {
    }
}
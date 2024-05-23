using System.Collections.Generic;

namespace CRA.DataMigration.Core.Services
{
    public interface IMetricService
    {
        void Increment<TMessage>(string statName, List<string> tags);
        void Increment(string statName, List<string> tags = null);
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace CRA.DataMigration.Core.Services
{
    public class MetricService : IMetricService
    {
        private readonly ILogger<MetricService> _logger;

        private readonly IDogStatsd _dogStatsd;

        public MetricService(IDogStatsd dogStatsd, ILogger<MetricService> logger)
        {
            _dogStatsd = dogStatsd;
            _logger = logger;
        }

        public void Increment<TMessage>(string statName, List<string> tags)
        {
            try
            {
                tags ??= new List<string>();

                tags.Add($"type:{typeof(TMessage).Name}");

                _dogStatsd.Increment(statName, tags: tags.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to increment metric, {@error}, {@statName} {@tags}", ex.Message, statName,
                    tags);
            }
        }

        public void Increment(string statName, List<string> tags = null)
        {
            try
            {
                tags ??= new List<string>();

                _dogStatsd.Increment(statName, tags: tags.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to increment metric, {@error}, {@statName} {@tags}", ex.Message, statName,
                    tags);
            }
        }
    }
}
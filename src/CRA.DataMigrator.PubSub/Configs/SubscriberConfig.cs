using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRA.DataMigrator.PubSub.Configs
{
    public class SubscriberConfig<TMessage> : ISubscriberConfiguration<TMessage>
        where TMessage : class, IMessageWithMetadata, new()
    {
        public bool Enable { get; set; }

        public bool UseEmulator { get; set; }

        public string ProjectId { get; set; }

        public string SubscriptionId { get; set; }

        public int? SubscriberClientCount { get; set; }

        public string Endpoint { get; set; }

        public TimeSpan AcknowledgeDeadline { get; set; }

        public TimeSpan AcknowledgeExtensionWindow { get; set; }

        public long MaximumOutstandingElementCount { get; set; }

        public long MaximumOutstandingByteCount { get; set; }

        public bool ShouldForwardErrors { get; set; }

        public string UniqueIdentifier { get; set; }

        public bool ShowPii { get; set; }

        public HealthStatus UnhealthyStateType { get; set; }

        public TimeSpan UnhealthyStateDuration { get; set; }

        public bool ShouldRestartSubscriberOnFailure { get; set; }

        public bool CreateSubscriptionWhenUsingEmulator { get; set; }

        public string TopicNameToCreateOnEmulator { get; set; }

        public Task HandleMessageLogAsync(
            ILogger logger,
            LogLevel logLevel,
            TMessage message,
            string logMessage,
            CancellationToken cancellationToken = new CancellationToken())
        {
            logger.Log(
                logLevel,
                "{UniqueIdentifier}:{SubscriptionId} -> {LogMessage}",
                UniqueIdentifier,
                SubscriptionId,
                logMessage);

            return Task.CompletedTask;
        }

        public Task HandleMessageLogAsync(ILogger logger, LogLevel logLevel, TMessage message, string logMessage, Exception error = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
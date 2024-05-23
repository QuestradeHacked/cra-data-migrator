using System;
using System.Threading.Tasks;
using CRA.DataMigrator.PubSub.Configs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Primitives;
using Questrade.Library.PubSubClientHelper.Subscriber.Default;

namespace CRA.DataMigrator.PubSub.Handlers
{
    public class ErrorHandler<TMessage> : IErrorHandler<TMessage>
        where TMessage : class, IMessageWithMetadata
    {
        private readonly int _maxRetryCount;
        private readonly int _failedQueueMessagesCacheTimeInMinutes;

        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ErrorHandler<TMessage>> _logger;

        public ErrorHandler(
            ILogger<ErrorHandler<TMessage>> logger,
            ErrorHandlerConfig errorHandlerConfig,
            IMemoryCache memoryCache
        )
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _maxRetryCount = errorHandlerConfig.MaxRetryCount;
            _failedQueueMessagesCacheTimeInMinutes = errorHandlerConfig.FailedQueueMessagesCacheTimeInMinutes;
        }

        public async Task<bool> AcknowledgeOnError(Exception exception, TMessage message)
        {
            var messageId = message.Id;
            var numberOfFailures = await _memoryCache.GetOrCreateAsync(messageId, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_failedQueueMessagesCacheTimeInMinutes));

                // set initial value in case entry not found
                return Task.FromResult(0);
            });


            if (numberOfFailures >= _maxRetryCount)
            {
                _logger.LogWarning("Number of failures reached the threshold point. Ack the message");

                return true;
            }

            _memoryCache.Set(messageId, ++numberOfFailures);

            _logger.LogWarning(
                $"Number of failures: {numberOfFailures}. the message will be retried until the threshold value is reached");

            return false;
        }
    }
}
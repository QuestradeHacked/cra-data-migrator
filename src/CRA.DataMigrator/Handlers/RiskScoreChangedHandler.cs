using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRA.DataMigration.Core.Constants;
using CRA.DataMigration.DAL.Entities.BigQuery;
using CRA.DataMigration.DAL.Repositories.BigQuery;
using CRA.DataMigrator.Managers.Abstract;
using CRA.DataMigrator.Models.Messages;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Subscriber.Default;

namespace CRA.DataMigrator.Handlers
{
    public class RiskScoreChangedHandler : IMessageHandler<RiskScoreChangedMessage>
    {
        private readonly ILogger<RiskScoreChangedHandler> _logger;
        private readonly ICustomerManager _customerManager;
        private readonly IMessageManager _messageManager;
        private readonly IBigQueryRepository _repository;
        private readonly IMapper _mapper;

        public RiskScoreChangedHandler(
            ILogger<RiskScoreChangedHandler> logger,
            ICustomerManager customerManager, IBigQueryRepository repository, IMapper mapper,
            IMessageManager messageManager)
        {
            _logger = logger;
            _customerManager = customerManager;
            _repository = repository;
            _mapper = mapper;
            _messageManager = messageManager;
        }

        public async Task<bool> HandleMessageAsync(
            RiskScoreChangedMessage message,
            string trackingId,
            CancellationToken cancellationToken = default)
        {
            var scope = new Dictionary<string, string>
            {
                {LoggingScopeKeys.MessageIdKey, message.Id},
                {LoggingScopeKeys.CustomerIdKey, message.Data?.CustomerId}
            };

            _logger.BeginScope(scope);
            _logger.LogInformation("Message received for {HandleMessage}", nameof(RiskScoreChangedHandler));


            var isProcessed = await _messageManager.IsProcessedAsync(message.Id, cancellationToken);
            if (isProcessed)
            {
                _logger.LogInformation("A duplicated message is received. Message is discarded");

                return true;
            }

            var data = message.Data;
            if (data == null)
            {
                _logger.LogWarning("A message with empty data was received");

                return true;
            }

            var riskRatingEntity = _mapper.Map<RiskRatingEntity>(data);

            var customer = await _customerManager.FindByIdAsync(data.CustomerId, cancellationToken);

            riskRatingEntity.CustomerFullName = $"{customer.FirstName} {customer.LastName}";

            await _repository.AddRecordAsync(riskRatingEntity);

            await _messageManager.AddMessageAsync(message.Id, cancellationToken);

            _logger.LogInformation("Message processed.");

            return true;
        }
    }
}
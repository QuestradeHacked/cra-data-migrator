using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRA.DataMigration.DAL.Entities.BigQuery;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.BigQuery;
using CRA.DataMigrator.Handlers;
using CRA.DataMigrator.Managers.Abstract;
using CRA.DataMigrator.Models.Data;
using CRA.DataMigrator.Models.Messages;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CRA.DataMigrator.UnitTests.Handlers;

public class RiskScoreChangedHandlerTest
{
    public RiskScoreChangedHandlerTest()
    {
        var loggerMock = new Mock<ILogger<RiskScoreChangedHandler>>();
        var bigQueryRepository = new Mock<IBigQueryRepository>();

        _mapper = new Mock<IMapper>();
        _messageManager = new Mock<IMessageManager>();
        _customManager = new Mock<ICustomerManager>();

        _riskScoreChangedHandler = new RiskScoreChangedHandler(
            loggerMock.Object,
            _customManager.Object,
            bigQueryRepository.Object,
            _mapper.Object,
            _messageManager.Object
        );
    }

    private readonly RiskScoreChangedHandler _riskScoreChangedHandler;
    private readonly Mock<IMessageManager> _messageManager;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ICustomerManager> _customManager;

    [Fact]
    public async Task HandleMessageAsync_ReturnsTrue_WhenMessageIsCorrect()
    {
        // Arrange
        var changedMessage = new RiskScoreChangedMessage
        {
            Data = new RiskScoreChangedData()
        };

        const bool isProcessed = false;

        _messageManager
            .Setup(message => message.IsProcessedAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(isProcessed);

        _mapper
            .Setup(map => map.Map<RiskRatingEntity>(It.IsAny<RiskScoreChangedData>()))
            .Returns(new RiskRatingEntity());

        _customManager
            .Setup(custom => custom.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { FirstName = "First Name", LastName = "Last Name" });

        // Act
        var response = await _riskScoreChangedHandler.HandleMessageAsync(changedMessage,It.IsAny<string>(), CancellationToken.None);

        // Assert
        response.Should().BeTrue();
    }

    [Fact]
    public async Task HandleMessageAsync_ReturnsTrue_WhenMessageIsAlreadyProcessed()
    {
        // Arrange
        const bool isProcessed = true;
        var changedMessage = new RiskScoreChangedMessage();

        _messageManager
            .Setup(message => message.IsProcessedAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(isProcessed);

        // Act
        var response = await _riskScoreChangedHandler.HandleMessageAsync(changedMessage,It.IsAny<string>(), CancellationToken.None);

        // Assert
        response.Should().BeTrue();
    }

    [Fact]
    public async Task HandleMessageAsync_ReturnsTrue_WhenDataIsNull()
    {
        // Arrange
        var changedMessage = new RiskScoreChangedMessage();

        // Act
        var response = await _riskScoreChangedHandler.HandleMessageAsync(changedMessage,It.IsAny<string>(), CancellationToken.None);

        // Assert
        response.Should().BeTrue();
    }
}
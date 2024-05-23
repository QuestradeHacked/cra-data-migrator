using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.Core.Services;
using CRA.DataMigration.DAL.Clients.BigQuery;
using CRA.DataMigration.DAL.Entities.BigQuery;
using CRA.DataMigration.DAL.Repositories.BigQuery;
using FluentAssertions;
using Google;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CRA.DataMigrator.UnitTests.Repositories
{
    public class BigQueryRepositoryTests
    {
        private readonly Mock<ICraBigQueryClient> _clientMock;
        private readonly Mock<ILogger<BigQueryRepository>> _loggerMock;
        private readonly Mock<IMetricService> metricServiceMock;

        readonly RiskRatingEntity _data = new RiskRatingEntity
        {
            CustomerId = "3000603", Rating = "High", PreviousRating = "Medium",
            RatingTime = DateTime.Now.AddDays(-5).ToUniversalTime().ToString(), TotalPoints = 120,
            Factors = new[]
            {
                new RiskFactorEntity()
                {
                    FactorName = "Country", FactorValue = "Algeria - Medium", Points = 5,
                    PreviousFactorValue = "Canada - Low"
                },
                new RiskFactorEntity()
                {
                    FactorName = "Occupation", FactorValue = "Programmer - Medium", Points = 5,
                    PreviousFactorValue = "Taxi driver - High"
                },
                new RiskFactorEntity()
                {
                    FactorName = "AccountCreationDate", FactorValue = "06/01/2015 00:00:00", Points = 1,
                    PreviousFactorValue = "06/01/2013 00:00:00"
                },
                new RiskFactorEntity()
                {
                    FactorName = "IsPoliticallyExposed", FactorValue = "False", Points = 0,
                    PreviousFactorValue = "False"
                },
            }
        };

        private readonly BigQueryRepository _repository;

        public BigQueryRepositoryTests()
        {
            _clientMock = new Mock<ICraBigQueryClient>();
            _loggerMock = new Mock<ILogger<BigQueryRepository>>();
            metricServiceMock = new Mock<IMetricService>();

            _repository = new BigQueryRepository(_loggerMock.Object, metricServiceMock.Object, _clientMock.Object);
        }

        [Fact]
        public async Task GddRecordAsync_NullPassed_ThrowsException()
        {
            var action = () => _repository.AddRecordAsync(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddRecordAsync_InsertingRowExceptionOccurs_Rethrown()
        {
            _clientMock.Setup(c => c.InsertRowAsync(It.IsAny<BigQueryInsertRow>(), It.IsAny<CancellationToken>()))
                .Throws(new GoogleApiException("testService", "testMessage"));

            var action = () => _repository.AddRecordAsync(_data);

            await action.Should().ThrowAsync<GoogleApiException>();
            metricServiceMock.Verify(l => l.Increment(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never());
        }

        [Fact]
        public async Task AddRecordAsync_ValidDataProvided_NoErrorsLogged()
        {
            await _repository.AddRecordAsync(_data);

            metricServiceMock.Verify(l => l.Increment(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Once());
        }
    }
}
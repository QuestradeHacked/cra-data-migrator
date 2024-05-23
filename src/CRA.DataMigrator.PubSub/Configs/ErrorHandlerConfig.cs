namespace CRA.DataMigrator.PubSub.Configs;

public class ErrorHandlerConfig
{
    public int MaxRetryCount { get; set; }
    public int FailedQueueMessagesCacheTimeInMinutes { get; set; }
}
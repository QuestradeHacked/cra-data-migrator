namespace QT.Clients.CustomerMaster.Configs
{
    public class Resilience
    {
        public int RetryCount { get; set; }
        public int TimeBetweenRetryInMilliseconds { get; set; }
    }
}
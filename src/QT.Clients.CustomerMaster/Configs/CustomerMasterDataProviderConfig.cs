using System;

namespace QT.Clients.CustomerMaster.Configs
{
    public class CustomerMasterDataProviderConfig
    {
        public string Token { get; set; }
        public Uri BaseUrl { get; set; }
        public Resilience Resilience { get; set; }
    }
}
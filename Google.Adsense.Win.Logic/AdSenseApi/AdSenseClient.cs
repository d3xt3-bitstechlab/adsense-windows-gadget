using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.Apis.Adsense.v1;
using Google.Apis.Adsense.v1.Data;
using Google.Apis.Util;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    class AdSenseClient : IAdSenseClient
    {
        private readonly AdsenseService service;

        public AdSenseClient(AdsenseService service)
        {
            service.ThrowIfNull("service");
        }

        public IList<string> FetchAdClients()
        {
            AdClients adclients = service.Adclients.List().Fetch();
            return (from client in adclients.Items
                    where client.SupportsReporting.HasValue && client.SupportsReporting.Value
                    select client.Id).ToList();  
        }

        public OverviewReport FetchOverview()
        {
            throw new NotImplementedException();
        }

        public ChannelSummary FetchCustomChannels()
        {
            throw new NotImplementedException();
        }

        public ChannelSummary FetchUrlChannels()
        {
            throw new NotImplementedException();
        }

        public AggregateRevenueSummary FetchYtdRevenue()
        {
            throw new NotImplementedException();
        }

        public AggregateRevenueSummary FetchLifetimeRevenue()
        {
            throw new NotImplementedException();
        }
    }
}

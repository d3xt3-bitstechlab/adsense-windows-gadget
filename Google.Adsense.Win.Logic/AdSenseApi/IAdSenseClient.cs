using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    interface IAdSenseClient
    {
      IList<string> FetchAdClients();

      OverviewReport FetchOverview();
  
      ChannelSummary FetchCustomChannels();
  
      ChannelSummary FetchUrlChannels();
  
      AggregateRevenueSummary FetchYtdRevenue();

      AggregateRevenueSummary FetchLifetimeRevenue();
    }

    class OverviewReport { }
    class ChannelSummary { }
    class AggregateRevenueSummary { }
}

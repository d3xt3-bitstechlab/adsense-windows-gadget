using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Google.Apis.Adsense.v1.Data;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    public class AdSenseZeroClient : IAdSenseClient
    {
        public IList<string> FetchAdClients()
        {
            return new List<string>().AsReadOnly();
        }

        public OverviewReport FetchOverview()
        {
            AdsenseReportsGenerateResponse response = new AdsenseReportsGenerateResponse();
            response.Rows = new List<IList<string>>();
            response.Rows.Add(new List<string>{"1970-01-01", "0", "0", "0", "0", "0", "0"});
            response.Headers = new List<AdsenseReportsGenerateResponse.HeadersData>{null, new AdsenseReportsGenerateResponse.HeadersData{Currency = "GBP"}};
            return new OverviewReport(CultureInfo.CurrentCulture, ReportDates.Today, 
                ReportDates.Yesterday, ReportDates.FirstOfLastMonth, ReportDates.FirstOfThisMonth, response);
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Google.Apis.Adsense.v1;
using Google.Apis.Adsense.v1.Data;
using Google.Apis.Util;

using NodaTime;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    public class AdSenseClient : IAdSenseClient
    {
        private readonly AdsenseService service;
        private readonly CultureInfo locale;

        public AdSenseClient(AdsenseService service, CultureInfo locale)
        {
            service.ThrowIfNull("service");
            locale.ThrowIfNull("locale");
            this.locale = locale;
            this.service = service;
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
            ZonedDateTime today = ReportDates.Today;
            ZonedDateTime sevenDayStart = today - new Duration(NodaConstants.TicksPerStandardDay * 6);

            String startDate = ReportDates.ToReportingString(sevenDayStart);
            String endDate = ReportDates.ToReportingString(today);
            var report = service.Reports.Generate(startDate, endDate);
            report.Locale = locale.TwoLetterISOLanguageName;
            report.Dimension = new Repeatable<string>(ChannelSummary.CustomeChannelDimensions);
            report.Metric = new Repeatable<string>(ChannelSummary.CustomerChannelMetrics);
            report.Sort = "-EARNINGS";
            report.MaxResults = 10;
            var result = report.Fetch();
            return new ChannelSummary(locale, 7, result);
        }


        public AggregateRevenueSummary FetchYtdRevenue()
        {
            throw new NotImplementedException();
        }

        public AggregateRevenueSummary FetchLifetimeRevenue()
        {
            throw new NotImplementedException();
        }

        public ChannelSummary FetchUrlChannels()
        {
            throw new NotImplementedException();
        }
    }
}

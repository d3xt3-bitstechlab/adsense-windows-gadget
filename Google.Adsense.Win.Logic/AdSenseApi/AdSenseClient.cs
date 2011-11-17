using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Google.Apis.Adsense.v1;
using Google.Apis.Adsense.v1.Data;
using Google.Apis.Util;

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
            //throw new NotImplementedException();
            
            // Calculate some important values of the date ranges we're working with.
            DateTime today = ReportDates.Today;
            DateTime yesterday = ReportDates.Yesterday;
            DateTime firstOfLastMonth = ReportDates.FirstOfLastMonth;
            DateTime firstOfThisMonth = ReportDates.FirstOfThisMonth;

            // Report on the whole of the current month and the whole of the previous month
            // by setting the start date to the first of the previous month.
            string startDate = ReportDates.ToReportingString(firstOfLastMonth);
            string endDate = ReportDates.ToReportingString(today);
            var report = service.Reports.Generate(startDate, endDate);
            report.Locale = locale.TwoLetterISOLanguageName;
            report.Dimension = new Repeatable<string>(OverviewReport.OverviewDimensions);
            report.Metric = new Repeatable<string>(OverviewReport.OverviewMetrics);
            report.Sort = "+DATE";
            var result = report.Fetch();
            
            return new OverviewReport(locale, today, yesterday, firstOfLastMonth, firstOfThisMonth, result);
        }

        public ChannelSummary FetchCustomChannels()
        {
            DateTime today = ReportDates.Today;
            DateTime sevenDayStart = today.AddDays(-6);

            string startDate = ReportDates.ToReportingString(sevenDayStart);
            string endDate = ReportDates.ToReportingString(today);
            var report = service.Reports.Generate(startDate, endDate);
            report.Locale = locale.TwoLetterISOLanguageName;
            report.Dimension = new Repeatable<string>(ChannelSummary.CustomeChannelDimensions);
            report.Metric = new Repeatable<string>(ChannelSummary.CustomerChannelMetrics);
            report.Sort = "-EARNINGS";
            report.MaxResults = 10;
            var result = report.Fetch();
            return new ChannelSummary(locale, 7, result);
        }

        public ChannelSummary FetchUrlChannels()
        {
            DateTime today = ReportDates.Today;
            DateTime sevenDayStart = today.AddDays(-6);

            string startDate = ReportDates.ToReportingString(sevenDayStart);
            string endDate = ReportDates.ToReportingString(today);
            var report = service.Reports.Generate(startDate, endDate);
            report.Locale = locale.TwoLetterISOLanguageName;
            report.Dimension = new Repeatable<string>(ChannelSummary.UrlChannelDimensions);
            report.Metric = new Repeatable<string>(ChannelSummary.UrlChannelMetrics);
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

    }
}

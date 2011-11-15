using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Google.Apis.Adsense.v1.Data;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    public class OverviewReport
    {
        private enum ReportResult
        {
            Date = 0,
            Earnings,
            PageViews,
            Clicks,
            ClickThroughRate,
            CostPerClick,
            RevenuePerMilli
        }

        public static readonly IList<string> OverviewDimensions = new List<string> {"DATE"}.AsReadOnly();
	    public static readonly IList<string> OverviewMetrics = new List<string> {
            "EARNINGS", "PAGE_VIEWS", "CLICKS", "PAGE_VIEWS_CTR", "COST_PER_CLICK", 
            "PAGE_VIEWS_RPM"}.AsReadOnly();

        private readonly System.Globalization.CultureInfo locale;
        private readonly DateTime today;
        private readonly DateTime yesterday;
        private readonly DateTime firstOfLastMonth;
        private readonly string currency;
        private readonly IDictionary<DateTime, OverviewReportResult> results;

        public System.Globalization.CultureInfo Locale { get { return locale; } }
        public string Currency { get { return currency; } }
        public IDictionary<DateTime, OverviewReportResult> Results { get { return results; } }

        public OverviewReport(CultureInfo locale, DateTime today, DateTime yesterday,
            DateTime firstOfLastMonth, AdsenseReportsGenerateResponse result)
        {
            this.locale = locale;
            this.today = today;
            this.yesterday = yesterday;
            this.firstOfLastMonth = firstOfLastMonth;
            this.currency = result.Headers[(int)ReportResult.Earnings].Currency;
            this.results = (from row in result.Rows
                            select new OverviewReportResult(row))
                            .ToDictionary(r => r.Date);
        }

        public struct OverviewReportResult
        {
            public DateTime Date { get; private set; }
            public double Earnings { get; private set; }
            public int PageViews { get; private set; }
            public int Clicks { get; private set; }
            public double ClickThroughRate { get; private set; }
            public double CostPerClick { get; private set; }
            public double RevenuePerMilli { get; private set; }

            public OverviewReportResult(IList<string> row): this()
            {
                DateTime date;
                double earnings;
                int pageViews;
                int clicks;
                double clickThroughRate;
                double costPerClick;
                double revenuePerMilli;
                if (DateTime.TryParse(row[(int)ReportResult.Date], out date))
                {
                    Date = date;
                }
                if (Double.TryParse(row[(int)ReportResult.Earnings], out earnings))
                {
                    Earnings = earnings;
                }
                if (Int32.TryParse(row[(int)ReportResult.PageViews], out pageViews))
                {
                    PageViews = pageViews;
                }
                if (Int32.TryParse(row[(int)ReportResult.Clicks], out clicks))
                {
                    Clicks = clicks;
                }
                if (Double.TryParse(row[(int)ReportResult.ClickThroughRate], out clickThroughRate))
                {
                    ClickThroughRate = clickThroughRate;
                }
                if (Double.TryParse(row[(int)ReportResult.CostPerClick], out costPerClick))
                {
                    CostPerClick = costPerClick;
                }
                if (Double.TryParse(row[(int)ReportResult.RevenuePerMilli], out revenuePerMilli))
                {
                    RevenuePerMilli = revenuePerMilli;
                }
            }
        }
    }
}

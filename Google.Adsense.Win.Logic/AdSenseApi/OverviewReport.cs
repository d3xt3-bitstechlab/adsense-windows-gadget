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
        private readonly DateTime firstOfThisMonth;
        private readonly string currency;
        private readonly IDictionary<DateTime, OverviewReportResult> results;
        private readonly OverviewReportResult monthToDate;
        private readonly OverviewReportResult lastMonth;
        private readonly OverviewReportResult resultToday;
        private readonly OverviewReportResult resultYesterday;
        
        public System.Globalization.CultureInfo Locale { get { return locale; } }
        public string Currency { get { return currency; } }
        public IDictionary<DateTime, OverviewReportResult> Results { get { return results; } }
        public OverviewReportResult Today { get { return resultToday; } }
        public OverviewReportResult Yesterday { get { return resultYesterday; } }
        public OverviewReportResult MonthToDate { get { return monthToDate; } }
        public OverviewReportResult LastMonth { get { return lastMonth; } }

        public OverviewReport(CultureInfo locale, DateTime today, DateTime yesterday,
            DateTime firstOfLastMonth, DateTime firstOfThisMonth, AdsenseReportsGenerateResponse result)
        {
            this.locale = locale;
            this.today = today;
            this.yesterday = yesterday;
            this.firstOfLastMonth = firstOfLastMonth;
            this.firstOfThisMonth = firstOfThisMonth;
            this.currency = result.Headers[(int)ReportResult.Earnings].Currency;
            this.results = (from row in result.Rows
                            select new OverviewReportResult(row))
                            .ToDictionary(r => r.Date);
            this.monthToDate = (from r in results.Values
                                where r.Date.CompareTo(firstOfThisMonth) >= 0
                                select r).Aggregate(OverviewReportResult.AggregateSeed(), OverviewReportResult.Aggregate);
            this.lastMonth = (from r in results.Values
                                where r.Date.CompareTo(firstOfThisMonth) < 0
                                select r).Aggregate(OverviewReportResult.AggregateSeed(), OverviewReportResult.Aggregate);
            this.resultToday = (from r in results.Values
                              where r.Date.CompareTo(today) == 0
                              select r).Aggregate(OverviewReportResult.AggregateSeed(), OverviewReportResult.Aggregate);
            this.resultYesterday = (from r in results.Values
                              where r.Date.CompareTo(yesterday) == 0
                              select r).Aggregate(OverviewReportResult.AggregateSeed(), OverviewReportResult.Aggregate);
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
            public bool IsAggregate { get; private set; }
            public int DaysAggregated { get; private set; }

            public static OverviewReportResult Aggregate(OverviewReportResult runningTotal, OverviewReportResult next)
            {
                if (runningTotal.IsAggregate == false)
                {
                    throw new ArgumentException("Parameter must be an Aggregate, please use AggregateSeed","runningTotal");
                }
                runningTotal.Date = runningTotal.Date.CompareTo(next.Date) < 0 ? runningTotal.Date : next.Date;
                runningTotal.DaysAggregated += next.DaysAggregated;
                runningTotal.Earnings += next.Earnings;
                runningTotal.PageViews += next.PageViews;
                runningTotal.Clicks += next.Clicks;
                runningTotal.ClickThroughRate = runningTotal.PageViews > 0 ? 
                    (double)runningTotal.Clicks / (double)runningTotal.PageViews 
                    : 0;
                runningTotal.CostPerClick = runningTotal.Clicks > 0 ? runningTotal.Earnings / (double)runningTotal.Clicks
                    : 0;
                runningTotal.RevenuePerMilli = 1000 * runningTotal.ClickThroughRate * runningTotal.CostPerClick;
                return runningTotal;
            }

            public static OverviewReportResult AggregateSeed()
            {
                return new OverviewReportResult() { Date = DateTime.MaxValue, IsAggregate = true, DaysAggregated = 0 };
            }

            public OverviewReportResult(IList<string> row): this()
            {
                DateTime date;
                double earnings;
                int pageViews;
                int clicks;
                double clickThroughRate;
                double costPerClick;
                double revenuePerMilli;
                IsAggregate = false;
                DaysAggregated = 1;
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

            public override string ToString()
            {
                return string.Format("{0:yyyy-MM-dd}\t{1:C}\t{2}\t{3}\t{4:P2}\t{5:C}\t{6:F4}\t{7}\t{8}",
                        Date, Earnings, PageViews, Clicks, ClickThroughRate,
                        CostPerClick, RevenuePerMilli, IsAggregate, DaysAggregated);
            }
        }
    }
}

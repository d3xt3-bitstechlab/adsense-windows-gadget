using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Google.Apis.Adsense.v1.Data;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    public class ChannelSummary
    {
        public static readonly IList<string> CustomeChannelDimensions =
            new List<string> { "CUSTOM_CHANNEL_ID", "CUSTOM_CHANNEL_NAME" }.AsReadOnly();
        public static readonly IList<string> CustomerChannelMetrics = 
            new List<string> { "EARNINGS" }.AsReadOnly();
        
        private readonly CultureInfo locale;
        private readonly int days;
        private readonly IList<ChannelReportResult> channels;
        private readonly string currency;

        public int Days { get { return days; } }
        public CultureInfo Locale { get { return locale; } }
        public IList<ChannelReportResult> Channels { get { return channels; } }
        public string Currency { get { return currency; } }

        public ChannelSummary(System.Globalization.CultureInfo locale, int days, AdsenseReportsGenerateResponse result)
        {
            this.locale = locale;
            this.days = days;
            this.currency = result.Headers[2].Currency;
            if (result.Rows != null)
            {
                channels = (from row in result.Rows
                            select new ChannelReportResult(row[0], row[1], row[2])).ToList().AsReadOnly();
            }
            else
            {
                channels = new List<ChannelReportResult>(0).AsReadOnly();
            }
        }

        public struct ChannelReportResult
        {
            public string Id { get; private set; }
            public string Name { get; private set; }
            public double? Earnings { get; private set; }

            public ChannelReportResult(string id, string name, string earnings): this()
            {
                this.Id = id;
                this.Name = name;
                double parsedEarnings;
                if (Double.TryParse(earnings, out parsedEarnings))
                {
                    this.Earnings = parsedEarnings;
                }
                else
                {
                    this.Earnings = null;
                }
            }
        }
    }
}

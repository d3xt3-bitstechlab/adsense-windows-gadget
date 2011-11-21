/*
Copyright 2011 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
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
        public static readonly IList<String> UrlChannelDimensions =
          new List<string> { "URL_CHANNEL_ID", "URL_CHANNEL_NAME" }.AsReadOnly();
        public static readonly IList<String> UrlChannelMetrics =
          new List<string> { "EARNINGS" }.AsReadOnly();

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

            public ChannelReportResult(string id, string name, string earnings)
                : this()
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

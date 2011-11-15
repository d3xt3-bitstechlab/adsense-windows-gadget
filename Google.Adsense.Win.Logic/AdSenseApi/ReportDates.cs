using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NodaTime;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    class ReportDates
    {
        static readonly DateTimeZone GoogleTimeZone = DateTimeZone.ForId("America/Los_Angeles");

        public static ZonedDateTime Today
        {
            get
            {
                return new ZonedDateTime(new Instant(DateTime.UtcNow.Ticks), GoogleTimeZone);
            }
        }

        public static string ToReportingString(ZonedDateTime date)
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2}", date.Year, date.MonthOfYear, date.DayOfMonth);
        }
    }
}

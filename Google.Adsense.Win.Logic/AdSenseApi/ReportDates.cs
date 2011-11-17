using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Google.Adsense.Win.Logic.AdSenseApi
{
    class ReportDates
    {
        public static DateTime Today
        {
            get
            {
                return DateTime.Today;
            }
        }

        public static DateTime Yesterday
        {
            get
            {
                DateTime yesterday = DateTime.Today.AddDays(-1);
                
                return yesterday;
            }
        }

        public static DateTime FirstOfLastMonth
        {
            get
            {
                DateTime today = Today;
                int day = 1;
                int month = today.Month - 1;
                int year = today.Year;
                if (month < 1)
                {
                    month = 1;
                    year--;
                }
                return new DateTime(year, month, day);
            }
        }

        public static string ToReportingString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
            //return string.Format("{0:D4}-{1:D2}-{2:D2}", date.Year, date.Month, date.Day);
        }

        public static DateTime FirstOfThisMonth
        {
            get
            {
                DateTime today = Today;
                return new DateTime(today.Year, today.Month, 1);
            }
        }
    }
}

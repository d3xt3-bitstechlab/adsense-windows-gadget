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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

using Google.Apis.Util;
using Google.Adsense.Win.Logic.AdSenseApi;

namespace Google.Adsense.Win.Gadget.ViewModel
{
    internal class OverviewSummaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool currentlyRefreshing = false;


        public IAdSenseClient AdSenseClient { get; set; }

        public OverviewSummaryViewModel(): base()
        {
            DateReportFetched = new DateTime(1970, 1, 1);
            Report = new AdSenseZeroClient().FetchOverview();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        private static OverviewSummaryViewModel instance = new OverviewSummaryViewModel();

        public static OverviewSummaryViewModel GetInstance()
        {
            return instance;
        }

        public void RefreshReport()
        {
            CurrentlyRefreshing = true;
            try
            {
                Report = AdSenseClient.FetchOverview();
                DateReportFetched = DateTime.Now;
            }
            finally
            {
                CurrentlyRefreshing = false;
            }
        }

        public bool CanRefreshReport
        {
            get { return currentlyRefreshing == false; }
        }

        #region Properties

        public bool CurrentlyRefreshing
        {
            get
            {
                return currentlyRefreshing;
            }
            set
            {
                this.currentlyRefreshing = value;
                OnPropertyChanged("CurrentlyRefreshing");
                OnPropertyChanged("CanRefreshReport");
            }
        }

        private DateTime dateReportFetched;
        public DateTime DateReportFetched
        {
            get { return dateReportFetched; }
            set
            {
                this.dateReportFetched = value;
                OnPropertyChanged("DateReportFetched");
            }
        }

        private OverviewReport report;
        public OverviewReport Report
        {
            get { return report; }
            set
            {
                this.report = value;
                OnPropertyChanged("Report");
            }
        }
        #endregion
    }
}

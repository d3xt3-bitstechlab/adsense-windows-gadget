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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Google.Adsense.Win.Logic;
using Google.Adsense.Win.Gadget.ViewModel;
using Google.Apis.Adsense.v1;


using DotNetOpenAuth.OAuth2;
using Google.Adsense.Win.Logic.AdSenseApi;

namespace Google.Adsense.Win.Gadget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IAdSenseClient realService;
        private IAdSenseClient zeroService;
        
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new Window();
            MainWindow.Background = System.Windows.Media.Brushes.Transparent;
            MainWindow.AllowsTransparency = true;
            MainWindow.WindowStyle = WindowStyle.None;
            MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainWindow.Content = new MainWindow();
            MainWindow.SizeToContent = SizeToContent.WidthAndHeight;
            MainWindow.MouseLeftButtonDown += delegate
             {
                 MainWindow.DragMove();
             };
            Configure();
            MainWindow.ShowDialog();
        }

        private void Configure()
        {
            IAuthenticatorProvider authProvider = new AdSenseAuthenticatorProvider(getConfirmationCodeFromUser);
            var service = new AdsenseService(authProvider.GetAuthenticator());
            realService = new AdSenseClient(service, System.Globalization.CultureInfo.CurrentUICulture);
            zeroService = new AdSenseZeroClient();
            OverviewSummaryViewModel.GetInstance().AdSenseClient = realService;
        }

        public static string getConfirmationCodeFromUser(Uri authUri)
        {
            //TODO(davidwaters) Implement this method.
            throw new NotImplementedException("WPF Conformation code retriver not yet done.");
        }
    }
}

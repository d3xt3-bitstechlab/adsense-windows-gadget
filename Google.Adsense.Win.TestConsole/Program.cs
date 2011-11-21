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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Google.Adsense.Win.Logic;
using Google.Adsense.Win.Logic.AdSenseApi;
using Google.Apis.Adsense.v1;
using Google.Apis.Adsense.v1.Data;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Util;

using DotNetOpenAuth.OAuth2;

namespace Google.Adsense.Win.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                simplerHelloWorld();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Well that didn't work.");
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("All Done. Press any key to continue...");
            Console.ReadLine();
        }

        private static void simplerHelloWorld()
        {
            IAuthenticatorProvider authProvider = new AdSenseAuthenticatorProvider(getConfirmationCodeFromUser);

            var service = new AdsenseService(authProvider.GetAuthenticator());
            for (int i = 0; i < 1; i++)
            {
                AdSenseClient client = new AdSenseClient(service, System.Globalization.CultureInfo.CurrentUICulture);
                IList<string> adClients = client.FetchAdClients();
                foreach (var currClient in adClients)
                {
                    Console.WriteLine(currClient);
                }

                Console.WriteLine("Custom Channels:");
                ChannelSummary customChannel = client.FetchCustomChannels();
                Console.WriteLine("Reporting Currency:" + customChannel.Currency);
                
                foreach (var channel in customChannel.Channels)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", channel.Id, channel.Name, channel.Earnings);
                }

                Console.WriteLine("Url Channels:");
                ChannelSummary urlChannel = client.FetchUrlChannels();
                Console.WriteLine("Reporting Currency:" + customChannel.Currency);
                foreach (var channel in urlChannel.Channels)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", channel.Id, channel.Name, channel.Earnings);
                }

                Console.WriteLine("Overview:");
                OverviewReport overview = client.FetchOverview();
                Console.WriteLine("Date\t\tGBP\tView\tClicks\tCTR\tCPC\tRPM\tAgg\tDays");
                foreach (var dayResult in overview.Results)
                {
                    var result = dayResult.Value;
                    Console.WriteLine(result);
                }
                Console.WriteLine("Last Month:");
                Console.WriteLine(overview.LastMonth);
                Console.WriteLine("Month To Date:");
                Console.WriteLine(overview.MonthToDate);

                Thread.Sleep(1000);
            }
        }

        private static void WriteDeveloperKey() 
        {
            string apiKey = "[API KEY]";
            string clientId = "[Client ID]";
            string clientSecret = "[Client Secret]";
            InsecureDeveloperKeyProvider.WriteDeveloperKey(apiKey, clientId, clientSecret);
        }

        public static string getConfirmationCodeFromUser(Uri authUri) 
        {
            Process.Start(authUri.ToString());
            Console.Write("  Authorization Code: ");
            string authCode = Console.ReadLine();
            Console.WriteLine();
            return authCode;
        }
    }
}

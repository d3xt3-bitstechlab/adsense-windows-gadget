using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Google.Adsense.Win.Logic;
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
            for (int i = 0; i < 5; i++)
            {
                AdClients clients = service.Adclients.List().Fetch();
                foreach (var currClient in clients.Items)
                {
                    Console.WriteLine(string.Format("{0}\t{1}\t{2}", currClient.Id, currClient.ProductCode, currClient.SupportsReporting));
                }
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

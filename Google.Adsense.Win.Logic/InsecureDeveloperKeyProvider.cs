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
using System.IO;
using System.Linq;
using System.Text;

using Google.Apis.Util;

namespace Google.Adsense.Win.Logic
{
    /// <summary>
    /// This class access developer key information stored in plain text, while sutable for development this is 
    /// not sutable for deployment. 
    /// </summary>
    public class InsecureDeveloperKeyProvider : IDeveloperKeyProvider
    {
        private const string ApplicationFolderName = "Google.Adsense.Win";
        private const string ClientCredentialsFileName = "client.dat";
        private const string FileKeyApiKey = "ApiKey";
        private const string FileKeyClientId = "ClientId";
        private const string FileKeyClientSecret = "ClientSecret";

        

        public InsecureDeveloperKeyProvider()
            : this(DefaultFile)
        {
        }

        public InsecureDeveloperKeyProvider(FileInfo file)
        {
            file.ThrowIfNull("file");
            if (file.Exists == false)
            {
                throw new ArgumentException(string.Format("File [{0}] must exist.",file.FullName));
            }
            IDictionary<string, string> values = ParseFile(file);
            if (values.Keys.Count < 3 ||
                values.ContainsKey(FileKeyApiKey) == false ||
                values.ContainsKey(FileKeyClientId) == false ||
                values.ContainsKey(FileKeyClientSecret) == false)
            {
                throw new ApplicationException(
                    String.Format("File [{0}] must contain at least the values [{1},{2},{3}] one per line in the form KEY=VALUE",
                        file.FullName, FileKeyApiKey, FileKeyClientId, FileKeyClientSecret));
            }
            ApiKey = values[FileKeyApiKey];
            ClientId = values[FileKeyClientId];
            ClientSecret = values[FileKeyClientSecret];
        }

        public static void WriteDeveloperKey(string apiKey, string clientId, string clientSecret)
        {
            if (DefaultDirectory.Exists == false)
            {
                DefaultDirectory.Create();
            }
            WriteDeveloperKey(DefaultFile, apiKey, clientId, clientSecret);
        }

        public static void WriteDeveloperKey(FileInfo file, string apiKey, string clientId, string clientSecret)
        {
            file.ThrowIfNull("file");
            using (FileStream fStream = file.OpenWrite())
            {
                using (TextWriter tw = new StreamWriter(fStream))
                {
                    tw.WriteLine("{0}={1}", FileKeyApiKey, apiKey);
                    tw.WriteLine("{0}={1}", FileKeyClientId, clientId);
                    tw.WriteLine("{0}={1}", FileKeyClientSecret, clientSecret);
                }
            }
        }

        private static FileInfo DefaultFile
        {
            get
            {
                return new FileInfo(Path.Combine(DefaultDirectory.FullName, ClientCredentialsFileName));
            }
        }

        private static DirectoryInfo DefaultDirectory
        {
            get
            {
                string applicationDate = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string googleAppDirectory = Path.Combine(applicationDate, ApplicationFolderName);
                return new DirectoryInfo(googleAppDirectory);
            }
        }

        private static IDictionary<string, string> ParseFile(FileInfo info)
        {
            var parsedValues = new Dictionary<string, string>(5);
            using (StreamReader sr = info.OpenText())
            {
                string currentLine = sr.ReadLine();
                while (currentLine != null)
                {
                    int firstEquals = currentLine.IndexOf('=');
                    if (firstEquals > 0 && firstEquals + 1 < currentLine.Length)
                    {
                        string key = currentLine.Substring(0, firstEquals).Trim();
                        string value = currentLine.Substring(firstEquals + 1).Trim();
                        parsedValues.Add(key, value);
                    }
                    currentLine = sr.ReadLine();
                }
            }
            return parsedValues;
        }


        public string ApiKey
        {
            get;
            private set;
        }

        public string ClientId
        {
            get;
            private set;
        }

        public string ClientSecret
        {
            get;
            private set;
        }
    }
}

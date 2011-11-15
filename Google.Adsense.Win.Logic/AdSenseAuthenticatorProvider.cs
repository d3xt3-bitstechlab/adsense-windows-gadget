using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IO;

using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Adsense.v1;
using Google.Apis.Util;

using DotNetOpenAuth.OAuth2;

namespace Google.Adsense.Win.Logic
{
    public class AdSenseAuthenticatorProvider : IAuthenticatorProvider
    {
        private enum AuthState : byte
        {
            NotPresent,
            Expired,
            Valid
        }

        private const string ApplicationFolderName = "Google.Adsense.Win";
        private const string AuthTokenFileName = "auth.dat";
        private const string FileKeyProtectedRefershToken = "RefreshToken";
        // Random data used to make this encryption key different from other information encyrpted with ProtectedData
        // This does not make it hard to decrypt just adds another small step.  
        private static readonly byte[] entropy = new byte[] {
         144, 161, 186, 39, 90, 101, 104, 99, 25, 9, 84, 208, 160, 228, 74, 133, 237, 167,
         115, 146, 10, 120, 23, 42, 53, 34, 254, 210, 40, 56, 4, 55, 187, 132, 46, 250,
         152, 201, 138, 78, 169, 157, 7, 130, 72, 107, 21, 118, 141, 12, 59, 135, 139, 109,
         131, 93, 16, 174, 99, 187, 240, 84, 108, 197, 247, 1, 168, 33, 30, 148, 90, 163,
         217, 104, 202, 208, 138, 72, 225, 166, 126, 247, 194, 250, 21, 177, 19, 68, 109, 
         126, 38, 10, 235, 121, 90, 150, 52, 63, 177 };

        private string refreshToken = null;
        private IDeveloperKeyProvider keys;
        private Func<Uri, string> getConfirmationCodeFromUser;
        private IAuthorizationState authorization;
        private FileInfo file;

        public AdSenseAuthenticatorProvider(Func<Uri, string> getConfirmationCodeFromUser)
            : this(getConfirmationCodeFromUser, DefaultFile, new InsecureDeveloperKeyProvider())
        {
        }

        public AdSenseAuthenticatorProvider(Func<Uri, string> getConfirmationCodeFromUser, FileInfo file, IDeveloperKeyProvider keys)
        {
            keys.ThrowIfNull("keys");
            file.ThrowIfNull("file");
            getConfirmationCodeFromUser.ThrowIfNull("userRequest");

            this.file = file;
            this.keys = keys;
            this.getConfirmationCodeFromUser = getConfirmationCodeFromUser;
            if (file.Exists)
            {
                IDictionary<string, string> values = ParseFile(file);
                if (values.Keys.Count >= 1 &&
                    values.ContainsKey(FileKeyProtectedRefershToken))
                {
                    refreshToken = Unprotect(values[FileKeyProtectedRefershToken]);
                }
            }
        }

        private static FileInfo DefaultFile
        {
            get
            {
                return new FileInfo(Path.Combine(DefaultDirectory.FullName, AuthTokenFileName));
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

        /// <summary>
        /// Encrypts the clearText using the current users key, this prevents other users being able to read this
        /// but does not stop the current user from reading this.
        /// </summary>
        private static string Protect(string clearText)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.ASCII.GetBytes(clearText), entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// The inverse of <code>Protect</code> this decrypts the passed-in string.
        /// </summary>
        private static string Unprotect(string encrypted)
        {
            byte[] encryptedData = Convert.FromBase64String(encrypted);
            byte[] clearText = ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
            return Encoding.ASCII.GetString(clearText);
        }

        public Apis.Authentication.IAuthenticator GetAuthenticator()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = keys.ClientId;
            provider.ClientSecret = keys.ClientSecret;
            return new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);   
        }

        private AuthState CurrentAuthorizationState()
        {
            if (authorization == null || 
                authorization.AccessToken.IsNullOrEmpty() || 
                authorization.AccessTokenExpirationUtc.HasValue == false)
            {
                return AuthState.NotPresent;
            }

            DateTime futre = DateTime.UtcNow.AddMinutes(5);
            // True if the expiration time is more then 5 minutes in the future 
            if (authorization.AccessTokenExpirationUtc.Value.CompareTo(futre) > 0)
            {
                return AuthState.Valid;
            }
            else
            {
                return AuthState.Expired;
            }
        }

        private IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            lock (this)
            {
                switch (CurrentAuthorizationState())
                {
                    case AuthState.NotPresent:
                        return CreateAuthorization(arg);
                    case AuthState.Expired:
                        if (arg.RefreshToken(authorization))
                        {
                            if (authorization.RefreshToken != refreshToken)
                            {
                                PersistRefreshToken(authorization.RefreshToken);
                            }
                            return authorization;
                        }
                        else
                        {
                            return CreateAuthorization(arg);
                        }
                    case AuthState.Valid:
                        return authorization;
                    default:
                        throw new ApplicationException("Unsupported AuthState " + CurrentAuthorizationState());
                }
            }
        }

        private IAuthorizationState CreateAuthorization(NativeApplicationClient arg)
        {
            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState(new[] { AdsenseService.Scopes.AdsenseReadonly.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            if (refreshToken.IsNotNullOrEmpty())
            {
                try
                {
                    state.RefreshToken = refreshToken;
                    if (arg.RefreshToken(state))
                    {
                        if (state.RefreshToken != refreshToken)
                        {
                            PersistRefreshToken(authorization.RefreshToken);
                        }
                        return this.authorization = state;
                    }
                }
                catch (DotNetOpenAuth.Messaging.ProtocolException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is System.Net.WebException)
                    {
                        System.Net.WebException webEx = ex.InnerException as System.Net.WebException;
                        HttpWebResponse response = webEx.Response as HttpWebResponse;
                        // if the refresh token is revoked by the user a http error code 400
                        // BadRequest is returned so we should remove this refreshtoken.
                        if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            //TODO Log the fact that this error occured.
                            state.RefreshToken = null;
                            refreshToken = null;
                            PersistRefreshToken("");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                
                state.RefreshToken = null;
                refreshToken = null;
                
            }

            Uri authUri = arg.RequestUserAuthorization(state);

            // Request authorization from the user.
            string authCode = getConfirmationCodeFromUser.Invoke(authUri);

            // Retrieve the access token by using the authorization code:
            IAuthorizationState authState = arg.ProcessUserAuthorization(authCode, state);
            PersistRefreshToken(state.RefreshToken);

            return this.authorization = authState;
        }

        private void PersistRefreshToken(string newToken)
        {
            refreshToken = newToken;
            if (file.Directory.Exists == false)
            {
                file.Directory.Create();
            }

            using (FileStream fStream = file.OpenWrite())
            {
                using (TextWriter tw = new StreamWriter(fStream))
                {
                    tw.WriteLine("{0}={1}", FileKeyProtectedRefershToken, Protect(refreshToken));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.Apis.Authentication;

namespace Google.Adsense.Win.Logic
{
    public interface IAuthenticatorProvider
    {
        IAuthenticator GetAuthenticator();
    }
}

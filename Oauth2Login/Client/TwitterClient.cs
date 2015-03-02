using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oauth2Login.Core;
using Oauth2Login.Service;

namespace Oauth2Login.Client
{
    public class TwitterClient : AbstractClientProvider
    {
        public TwitterClient()
        {
        }

        public TwitterClient(string oClientid, string oClientsecret, string oCallbackUrl, string oScope,
            string oAcceptedUrl, string oFailedUrl, string oProxy)
            : base(oClientid, oClientsecret, oCallbackUrl, oScope, oAcceptedUrl, oFailedUrl, oProxy)
        {
            ServiceType = typeof(TwitterService);
        }
    }
}

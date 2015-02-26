using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    public abstract class BaseOauth2Service : IClientService
    {
        protected AbstractClientProvider _client;

        protected BaseOauth2Service(AbstractClientProvider oClient)
        {
            _client = oClient;
        }

        public void CreateOAuthClient(IOAuthContext oContext)
        {
            _client = oContext.Client;
        }

        public void CreateOAuthClient(AbstractClientProvider oClient)
        {
            _client = oClient;
        }

        protected string HttpGet(string url)
        {
            var header = new NameValueCollection
            {
                {"Accept-Language", "en-US"}
            };

            return RestfullRequest.Request(url, "GET", "application/x-www-form-urlencoded", header, null, _client.Proxy);
        }

        protected string HttpPost(string urlToPost, string postData)
        {
            var result = RestfullRequest.Request(urlToPost, "POST", "application/x-www-form-urlencoded",
                    null, postData, _client.Proxy);

            return result;
        }

        // oh you abstract base class, leave something for children to implement
        public abstract string BeginAuthentication();
        public abstract string RequestToken();
        public abstract Dictionary<string, string> RequestUserProfile();
    }
}

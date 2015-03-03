using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Oauth2Login.Client;
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
        public abstract string RequestToken(HttpRequestBase request);
        public abstract Dictionary<string, string> RequestUserProfile();

        // TODO: This looks horrible, refactor using generics
        public static BaseOauth2Service GetService(string id)
        {
            switch (id.ToLower())
            {
                case "google":
                    return new GoogleService(Oauth2LoginFactory.CreateClient<GoogleClient>("Google"));
                    break;
                case "facebook":
                    return new FacebookService(Oauth2LoginFactory.CreateClient<FacebookClient>("Facebook"));
                    break;
                // Need to transition WindowLive to new base class
                //case "windowslive":
                //    return new WindowsLiveService(Oauth2LoginFactory.CreateClient<WindowsLiveClient>("WindowsLive"));
                //    break;
                case "paypal":
                    return new PayPalService(Oauth2LoginFactory.CreateClient<PayPalClient>("PayPal"));
                    break;
                case "twitter":
                    return new TwitterService(Oauth2LoginFactory.CreateClient<TwitterClient>("Twitter"));
                    break;
                default:
                    return null;
            }
        }


        public string ValidateLogin(HttpRequestBase request)
        {
            // client token
            string tokenResult = RequestToken(request);
            if (tokenResult == OAuth2Consts.ACCESS_DENIED)
                return _client.FailedRedirectUrl;

            _client.Token = tokenResult;

            // client profile
            Dictionary<string, string> result = RequestUserProfile();
            if (result != null)
                _client.Profile = result;
            else
                throw new Exception("ERROR: [Oauth2LoginContext] Profile is not found!");

            return null;
        }
    }
}

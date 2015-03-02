using System;
using System.Collections.Generic;
using Oauth2Login.Client;

namespace Oauth2Login.Core
{
    public abstract class AbstractClientProvider
    {
        protected AbstractClientProvider()
        {
        }

        protected AbstractClientProvider(string oClientid, string oClientsecret, string oCallbackUrl, string oScope,
            string oAcceptedUrl, string oFailedUrl, string oProxy)
        {
            ClientId = oClientid;
            ClientSecret = oClientsecret;
            CallBackUrl = oCallbackUrl;
            Scope = oScope;
            AcceptedRedirectUrl = oAcceptedUrl;
            FailedRedirectUrl = oFailedUrl;
            Proxy = oProxy;
            Token = "";
        }

        public Type ServiceType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallBackUrl { get; set; }
        public string Scope { get; set; }
        public string AcceptedRedirectUrl { get; set; }
        public string FailedRedirectUrl { get; set; }
        public string Proxy { get; set; }
        public string Token { get; set; }
        public Dictionary<string, string> Profile { get; set; }
        public string ProfileJsonString { get; set; }

        public static AbstractClientProvider ClientById(string id)
        {
            switch (id.ToLower())
            {
                case "google":
                    return Oauth2LoginFactory.CreateClient<GoogleClient>("Google");
                    break;
                case "facebook":
                    return Oauth2LoginFactory.CreateClient<FacebookClient>("Facebook");
                    break;
                case "windowslive":
                    return Oauth2LoginFactory.CreateClient<WindowsLiveClient>("WindowsLive");
                    break;
                case "paypal":
                    return Oauth2LoginFactory.CreateClient<PayPalClient>("PayPal");
                    break;
                case "twitter":
                    return Oauth2LoginFactory.CreateClient<TwitterClient>("Twitter");
                default:
                    return null;
            }
        }
    }
}
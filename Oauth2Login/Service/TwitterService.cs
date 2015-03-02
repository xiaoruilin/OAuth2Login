using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    // Twitter support is technically not Oauth2
    // 4 years worth of fun: https://twittercommunity.com/t/oauth-2-0-support/253
    public class TwitterService : BaseOauth2Service
    {
        private const string _twitterRequestTokenUrl = "https://api.twitter.com/oauth/request_token";
        private const string _twitterAccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        private const string _twitterAuthorizeUrl = "https://api.twitter.com/oauth/authorize";

        public TwitterService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            var util = new OAuth2Util();

            string nonce = util.GetNonce();
            string timeStamp = util.GetTimeStamp();

            var qstring = QueryStringBuilder.Build(
                "oauth_callback", _client.CallBackUrl,
                "oauth_consumer_key", _client.ClientId,
                "oauth_nonce", nonce,
                "oauth_signature_method", "HMAC-SHA1",
                "oauth_timestamp", timeStamp,
                "oauth_version", "1.0"
                );

            var sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(_twitterRequestTokenUrl) + "&" + Uri.EscapeDataString(qstring);

            string signature = util.GetSignature(sigBaseString, _client.ClientSecret);

            var responseText = HttpPost(_twitterRequestTokenUrl, qstring + "&oauth_signature=" + Uri.EscapeDataString(signature));

            var twitterAuthResp = new TwitterAuthResponse(responseText);
            var oauthUrl = "https://api.twitter.com/oauth/authenticate?oauth_token=" + twitterAuthResp.OAuthToken;

            return oauthUrl;
        }

        // TODO: Refactor
        public class TwitterAuthResponse
        {
            public string OAuthToken { get; set; }
            public string OAuthTokenSecret { get; set; }
            public bool OAuthCallbackConfirmed { get; set; }
            public string OAuthAuthorizeUrl { get; set; }

            public TwitterAuthResponse(string responseText)
            {
                string[] keyValPairs = responseText.Split('&');

                for (int i = 0; i < keyValPairs.Length; i++)
                {
                    String[] splits = keyValPairs[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            OAuthToken = splits[1];
                            break;
                        case "oauth_token_secret":
                            OAuthTokenSecret = splits[1];
                            break;
                        case "oauth_callback_confirmed":
                            OAuthCallbackConfirmed = splits[1] == "true";
                            break;
                        case "xoauth_request_auth_url":
                            OAuthAuthorizeUrl = splits[1];
                            break;
                    }
                }
            }
        }

        public override string RequestToken()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> RequestUserProfile()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Oauth2Login.Client;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    // Twitter support is technically not Oauth2
    // 4 years worth of fun: 
    // https://twittercommunity.com/t/oauth-2-0-support/253
    // https://twittercommunity.com/t/how-to-get-email-from-twitter-user-using-oauthtokens/558/155
    public class TwitterService : BaseOauth2Service
    {
        private readonly OAuth2Util _util = new OAuth2Util();

        public TwitterService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                // Twitter relies on Callback URL setting on http://apps.twitter.com/
                // If you set client callback here it'll return 401
                //                "oauth_callback", _client.CallBackUrl,
                "oauth_consumer_key", _client.ClientId,
                "oauth_nonce", _util.GetNonce(),
                "oauth_signature_method", "HMAC-SHA1",
                "oauth_timestamp", _util.GetTimeStamp(),
                "oauth_version", "1.0"
                );

            const string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
            var signature = getSha1Signature("POST", requestTokenUrl, qstring);
            var responseText = HttpPost(requestTokenUrl, qstring + "&oauth_signature=" + Uri.EscapeDataString(signature));

            var twitterAuthResp = new TwitterAuthResponse(responseText);

            const string authenticateUrl = "https://api.twitter.com/oauth/authenticate";
            var oauthUrl = authenticateUrl + "?oauth_token=" + twitterAuthResp.OAuthToken;

            return oauthUrl;
        }

        // TODO: Refactor
        public class TwitterAuthResponse
        {
            public string OAuthToken { get; set; }
            public string OAuthTokenSecret { get; set; }
            public bool OAuthCallbackConfirmed { get; set; }
            public string OAuthAuthorizeUrl { get; set; }
            public int UserId { get; set; }
            public string ScreenName { get; set; }

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
                        case "user_id":
                            UserId = int.Parse(splits[1]);
                            break;
                        case "screen_name":
                            ScreenName = splits[1];
                            break;
                    }
                }
            }
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var oauthToken = request["oauth_token"];
            var oauthVerifier = request["oauth_verifier"];

            if (String.IsNullOrEmpty(oauthToken) || String.IsNullOrEmpty(oauthVerifier))
                return OAuth2Consts.ACCESS_DENIED;

            string postData = QueryStringBuilder.Build(
                "oauth_consumer_key", _client.ClientId,
                "oauth_nonce", _util.GetNonce(),
                "oauth_signature_method", "HMAC-SHA1",
                "oauth_timestamp", _util.GetTimeStamp(),
                "oauth_token", oauthToken,
                "oauth_verifier", oauthVerifier,
                "oauth_version", "1.0"
                );

            const string accessTokenUrl = "https://api.twitter.com/oauth/access_token";
            var signature = getSha1Signature("POST", accessTokenUrl, postData);
            var responseText = HttpPost(accessTokenUrl, postData + "&oauth_signature=" + Uri.EscapeDataString(signature));

            var twitterAuthResp = new TwitterAuthResponse(responseText);

            _client.TokenSecret = twitterAuthResp.OAuthTokenSecret;

            return twitterAuthResp.OAuthToken;
        }

        public override Dictionary<string, string> RequestUserProfile()
        {
            string qstring = QueryStringBuilder.Build(
                "oauth_consumer_key", _client.ClientId,
                "oauth_nonce", _util.GetNonce(),
                "oauth_signature_method", "HMAC-SHA1",
                "oauth_timestamp", _util.GetTimeStamp(),
                "oauth_token", _client.Token,
                "oauth_version", "1.0"
                );

            const string profileUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
            var signature = getSha1Signature("GET", profileUrl, qstring, _client.TokenSecret);
            qstring += "&oauth_signature=" + Uri.EscapeDataString(signature);

            string result = HttpGet(profileUrl + "?" + qstring);
            _client.ProfileJsonString = result;

            // TODO: Use in future
            var data = JsonConvert.DeserializeAnonymousType(result, new TwitterClient.UserProfile());

            var flatString = JsonConvert.SerializeObject(data);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(flatString);
            return dictionary;
        }

        private string getSha1Signature(string httpMethod, string url, string data, string extraSigningKey = null)
        {
            var sigBaseString = httpMethod + "&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(data);
            string signature = _util.GetSignature(sigBaseString, _client.ClientSecret, extraSigningKey);

            return signature;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using Oauth2Login.Client;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    public class PayPalService : BaseOauth2Service
    {
        private static string _oauthUrl = "";

        private const string OAUTH_API_URL = "https://api.sandbox.paypal.com";
        //private const string OAUTH_API_URL = "https://api.paypal.com"; 

        private const string OAUTH_API_LOGIN_URL = "https://www.sandbox.paypal.com";
        //private const string OAUTH_API_LOGIN_URL = "https://www.paypal.com"; 

        public PayPalService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            if (_client != null)
            {
                var qstring = QueryStringBuilder.Build(
                    "client_id", _client.ClientId,
                    "response_type", "code",
                    "redirect_uri", _client.CallBackUrl,
                    "scope", _client.Scope
                    );

                _oauthUrl = OAUTH_API_LOGIN_URL + "/webapps/auth/protocol/openidconnect/v1/authorize?" + qstring;
                                          
                return _oauthUrl;
            }
            throw new Exception("ERROR: BeginAuth the client not found!");
        }

        public override string RequestToken()
        {
            string code = HttpContext.Current.Request.Params["code"];
            if (code != null)
            {
                const string oauthUrl = OAUTH_API_URL + "/v1/identity/openidconnect/tokenservice";
                var postData = QueryStringBuilder.Build(
                    "grant_type", "authorization_code",
                    "redirect_uri", _client.CallBackUrl,
                    "code", code,
                    "client_id", _client.ClientId,
                    "client_secret", _client.ClientSecret
                    );
                var responseJson = HttpPost(oauthUrl, postData);
                return JsonConvert.DeserializeAnonymousType(responseJson, new { access_token = "" }).access_token;
            }
            return Oauth2Consts.ACCESS_DENIED;
        }

        public override Dictionary<string, string> RequestUserProfile()
        {
            const string profileUrl = OAUTH_API_URL + "/v1/identity/openidconnect/userinfo/?schema=openid";

            var header = new NameValueCollection
            {
                {"Accept-Language", "en_US"},
                {"Authorization", "Bearer " + _client.Token}
            };
            string result = RestfullRequest.Request(profileUrl, "POST", "application/json", header, null, _client.Proxy);
            _client.ProfileJsonString = result;
            var data = JsonConvert.DeserializeAnonymousType(result, new PayPalClient.UserProfile());

            var dictionary = new Dictionary<string, string>
            {
                {"source", "PayPal"},
                {"email", data.Email},
                {"verified_email", data.Verified_email},
                {"name", data.Name},
                {"given_name", data.Given_name},
                {"family_name", data.Family_name},
                {"link", data.User_id},
                {"picture", data.Picture},
                {"gender", data.Gender}
            };
            return dictionary;
        }
    }
}
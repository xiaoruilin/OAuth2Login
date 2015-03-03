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

        private string ApiUrlOauth
        {
            get
            {
                return _client.Endpoint == OAuth2Consts.SANDBOX
                    ? "https://api.sandbox.paypal.com"
                    : "https://api.paypal.com";
            }
        }

        private string LoginUrlOauth
        {
            get
            {
                return _client.Endpoint == OAuth2Consts.SANDBOX
                    ? "https://www.sandbox.paypal.com"
                    : "https://www.paypal.com";
            }
        }

        public PayPalService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                "client_id", _client.ClientId,
                "response_type", "code",
                "redirect_uri", _client.CallBackUrl,
                "scope", _client.Scope
                );

            _oauthUrl = LoginUrlOauth + "/webapps/auth/protocol/openidconnect/v1/authorize?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED;

            var oauthUrl = ApiUrlOauth + "/v1/identity/openidconnect/tokenservice";
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

        public override Dictionary<string, string> RequestUserProfile()
        {
            var profileUrl = ApiUrlOauth + "/v1/identity/openidconnect/userinfo/?schema=openid";

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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
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

        public PayPalService(AbstractClientProvider oClient)
            : base(oClient)
        {
        }

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

        public override void RequestUserProfile()
        {
            var profileUrl = ApiUrlOauth + "/v1/identity/openidconnect/userinfo/?schema=openid";

            var header = new NameValueCollection
            {
                {"Accept-Language", "en_US"},
                {"Authorization", "Bearer " + _client.Token}
            };
            var result = RestfullRequest.Request(profileUrl, "POST", "application/json", header, null, _client.Proxy);

            ParseUserData<PayPalUserData>(result);
        }
    }

    public class PayPalUserData : BaseUserData
    {
        public PayPalUserData() : base(ExternalAuthServices.PayPal) { }

        public Address address { get; set; }
        public string Language { get; set; }
        public string Locale { get; set; }
        public string Zoneinfo { get; set; }
        public DateTime Birthday { get; set; }
        //public string Name { get; set; }
        public string Given_name { get; set; }
        public string Family_name { get; set; }
        public string Verified_email { get; set; }
        public string Gender { get; set; }
        public string Picture { get; set; }

        public class Address
        {
            public int Postal_code { get; set; }
            public string Locality { get; set; }
            public string Region { get; set; }
            public string Country { get; set; }
            public string Street_address { get; set; }
        }

        // override
        [DataMember(Name = "User_id")]
        public override string UserId { get; set; }

        [DataMember(Name = "Email")]
        public override string Email { get; set; }

        [DataMember(Name = "Phone_number")]
        public override string PhoneNumber { get; set; }

        [DataMember(Name = "Name")]
        public override string FullName { get; set; }
    }
}
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
    public class FacebookService : BaseOauth2Service
    {
        private static string _oauthUrl = "";

        public FacebookService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                "client_id", _client.ClientId,
                "redirect_uri", _client.CallBackUrl,
                "scope", _client.Scope,
                "state", "",
                "display", "popup"
                );

            _oauthUrl = "https://www.facebook.com/dialog/oauth?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED;

            string tokenUrl = string.Format("https://graph.facebook.com/oauth/access_token?");
            string postData = QueryStringBuilder.Build(
                "client_id", _client.ClientId,
                "redirect_uri", _client.CallBackUrl,
                "client_secret", _client.ClientSecret,
                "code", code
            );

            string resonseJson = HttpPost(tokenUrl, postData);
            resonseJson = "{\"" + resonseJson.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
            return JsonConvert.DeserializeAnonymousType(resonseJson, new { access_token = "" }).access_token;
        }

        public override void RequestUserProfile()
        {
            string profileUrl = "https://graph.facebook.com/me?access_token=" + _client.Token;

            string result = HttpGet(profileUrl);

            ParseUserData<FacebookUserData>(result);
        }
    }

    public class FacebookUserData : BaseUserData
    {
        public FacebookUserData() : base(ExternalAuthServices.Facebook) { }

        public string Name { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Link { get; set; }
        public string Gender { get; set; }
        public string Picture { get; set; }

        // override
        [DataMember(Name = "Id")]
        public override string UserId { get; set; }
        [DataMember(Name = "Email")]
        public override string Email { get; set; }
        [DataMember(Name = "xxx")] // not implemented
        public override string PhoneNumber { get; set; }
    }
}
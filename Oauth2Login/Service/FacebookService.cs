using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public override Dictionary<string, string> RequestUserProfile()
        {
            string profileUrl = "https://graph.facebook.com/me?access_token=" + _client.Token;

            string result = HttpGet(profileUrl);
            _client.ProfileJsonString = result;
            var data = JsonConvert.DeserializeAnonymousType(result, new FacebookClient.UserProfile());

            var dictionary = new Dictionary<string, string>
            {
                {"source", "Facebook"},
                {"id", data.Id},
                {"name", data.Name},
                {"first_name", data.First_name},
                {"last_name", data.Last_name},
                {"link", data.Link},
                {"gender", data.Gender},
                {"email", data.Email},
                {"picture", data.Picture}
            };
            return dictionary;
        }
    }
}
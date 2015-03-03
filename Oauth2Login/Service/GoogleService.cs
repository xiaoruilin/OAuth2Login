using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using Oauth2Login.Client;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    public class GoogleService : BaseOauth2Service
    {
        private static string _oauthUrl = "";

        public GoogleService(AbstractClientProvider oClient) : base(oClient) { }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.BuildCompex(new[] { "scope" },
                "scope", _client.Scope,
                "state", "1",
                "redirect_uri", _client.CallBackUrl,
                "client_id", _client.ClientId,
                "response_type", "code",
                "approval_prompt", "auto",
                "access_type", "online"
                );

            _oauthUrl = "https://accounts.google.com/o/oauth2/auth?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED;

            const string tokenUrl = "https://accounts.google.com/o/oauth2/token";
            var postData = QueryStringBuilder.Build(
                "code", code,
                "client_id", _client.ClientId,
                "client_secret", _client.ClientSecret,
                "redirect_uri", _client.CallBackUrl,
                "grant_type", "authorization_code"
                );

            string resonseJson = HttpPost(tokenUrl, postData);
            return JsonConvert.DeserializeAnonymousType(resonseJson, new { access_token = "" }).access_token;
        }

        public override Dictionary<string, string> RequestUserProfile()
        {
            string profileUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + _client.Token;

            string result = HttpGet(profileUrl);
            _client.ProfileJsonString = result;
            var data = JsonConvert.DeserializeAnonymousType(result, new GoogleClient.UserProfile());

            var dictionary = new Dictionary<string, string>
            {
                {"source", "Google"},
                {"id", data.Id},
                {"email", data.Email},
                {"verified_email", data.Verified_email},
                {"name", data.Name},
                {"given_name", data.Given_name},
                {"family_name", data.Family_name},
                {"link", data.Link},
                {"picture", data.Picture},
                {"gender", data.Gender}
            };
            return dictionary;
        }
    }
}
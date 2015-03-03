using Oauth2Login.Configuration;
using Oauth2Login.Core;
using Oauth2Login.Service;

namespace Oauth2Login.Client
{
    public class GoogleClient : AbstractClientProvider
    {
        public GoogleClient()
        {
        }

        public GoogleClient(OAuthWebConfigurationElement ccRoot, OAuthConfigurationElement ccOauth)
            : base(ccRoot, ccOauth)
        {
            //ServiceType = typeof (GoogleService);
        }

        public class UserProfile
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string Verified_email { get; set; }
            public string Name { get; set; }
            public string Given_name { get; set; }
            public string Family_name { get; set; }
            public string Link { get; set; }
            public string Picture { get; set; }
            public string Gender { get; set; }
        }
    }
}
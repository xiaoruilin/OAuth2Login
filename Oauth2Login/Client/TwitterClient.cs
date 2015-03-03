using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oauth2Login.Configuration;
using Oauth2Login.Core;
using Oauth2Login.Service;

namespace Oauth2Login.Client
{
    public class TwitterClient : AbstractClientProvider
    {
        public TwitterClient()
        {
        }

        public TwitterClient(OAuthWebConfigurationElement ccRoot, OAuthConfigurationElement ccOauth)
            : base(ccRoot, ccOauth)
        {
            //ServiceType = typeof(TwitterService);
        }

        public class UserProfile
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Screen_Name { get; set; }
            public string Location { get; set; }
            public string Profile_Location { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }

            public bool Protected { get; set; }
            public bool Verified { get; set; }

            public int Followers_Count { get; set; }
            public int Friends_Count { get; set; }
            public int Listed_Count { get; set; }
            public int Favourites_Count { get; set; }
            public int Statuses_Count { get; set; }

            public bool Following { get; set; }
            public bool Following_Request_Sent { get; set; }
            public bool Notifications { get; set; }
        }
    }
}

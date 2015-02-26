using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Oauth2Login.Configuration;

namespace Oauth2Login
{
    public class Oauth2LoginContext : IOAuthContext
    {
        private const string _sessionKey = "Oauth2LoginContext";
        private const string _cookieKey = "Oauth2LoginCookie";

        public Oauth2LoginContext()
        {
        }

        public Oauth2LoginContext(AbstractClientProvider oClient)
        {
            if (oClient != null)
            {
                Client = oClient;
                Service = (IClientService) Activator.CreateInstance(Client.ServiceType, new object[] {Client});
                HttpContext.Current.Session.Add(_sessionKey, this);
                var oauthCookie = new HttpCookie(_cookieKey);
                oauthCookie["configuration"] = Client.GetType().Name.Replace("Client", "");
                oauthCookie.Expires = DateTime.Now.AddHours(1);
                HttpContext.Current.Request.Cookies.Add(oauthCookie);
            }
            else
                throw new Exception("ERROR: [Oauth2LoginContext] Client is not found!");
        }

        public static Oauth2LoginContext Current
        {
            get
            {
                Oauth2LoginContext context = null;
                context = HttpContext.Current.Session[_sessionKey] as Oauth2LoginContext;

                if (context == null)
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[_cookieKey];
                    if (cookie != null)
                    {
                        var clientConfiguration =
                            ConfigurationManager.GetSection("oauth2.login.configuration") as OAuthConfigurationSection;
                        if (clientConfiguration != null)
                        {
                            IEnumerator configurationReader =
                                clientConfiguration.OAuthVClientConfigurations.GetEnumerator();
                            string acceptedUrl = clientConfiguration.WebConfiguration.AcceptedRedirectUrl;
                            string failedUrl = clientConfiguration.WebConfiguration.FailedRedirectUrl;
                            while (configurationReader.MoveNext())
                            {
                                if (configurationReader.Current is OAuthConfigurationElement)
                                {
                                    var clientConfigurationElement =
                                        configurationReader.Current as OAuthConfigurationElement;

                                    if (cookie["configuration"] != null)
                                    {
                                        if (clientConfigurationElement.Name == cookie["configuration"])
                                        {
                                            Type cType = Type.GetType(clientConfigurationElement.Name + "Client");
                                            var client =
                                                (AbstractClientProvider) Activator.CreateInstance(cType, new object[]
                                                {
                                                    cType,
                                                    clientConfigurationElement.ClientId,
                                                    clientConfigurationElement.ClientSecret,
                                                    clientConfigurationElement.CallbackUrl,
                                                    clientConfigurationElement.Scope,
                                                    acceptedUrl,
                                                    failedUrl,
                                                    clientConfigurationElement.Proxy
                                                });
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("ERROR: Cookie[configuration] is not found!");
                                    }
                                }
                            }
                        }
                    }
                }
                return context;
            }
        }

        public AbstractClientProvider Client { get; set; }
        public IClientService Service { get; set; }

        public string Token
        {
            get { return Client.Token; }
            set { Client.Token = value; }
        }

        public Dictionary<string, string> Profile
        {
            get { return Client.Profile; }
            set { Client.Profile = value; }
        }

        /// <summary>
        ///     验证开始
        /// </summary>
        public string BeginAuth()
        {
            return Service.BeginAuthentication();
        }

        public string ValidateLogin()
        {
            // client token
            string tokenResult = Service.RequestToken();
            if (tokenResult == Oauth2Consts.ACCESS_DENIED)
                return Client.FailedRedirectUrl;
            Client.Token = tokenResult;

            // client profile
            Dictionary<string, string> result = Service.RequestUserProfile();
            if (result != null)
                Client.Profile = result;
            else
                throw new Exception("ERROR: [Oauth2LoginContext] Profile is not found!");

            return null;
        }

        public static Oauth2LoginContext Create(AbstractClientProvider oClient)
        {
            return new Oauth2LoginContext(oClient);
        }
    }
}
using System;
using System.Collections;
using System.Configuration;
using Oauth2Login.Configuration;

namespace Oauth2Login.Core
{
    public class Oauth2LoginFactory
    {
        public static T CreateClient<T>(string oConfigurationName) where T : AbstractClientProvider, new()
        {
            var ccRoot =
                ConfigurationManager.GetSection("oauth2.login.configuration") as OAuthConfigurationSection;

            if (ccRoot != null)
            {
                var ccWebElem = ccRoot.WebConfiguration;

                IEnumerator configurationReader = ccRoot.OAuthVClientConfigurations.GetEnumerator();

                while (configurationReader.MoveNext())
                {
                    if (configurationReader.Current is OAuthConfigurationElement)
                    {
                        var clientConfigurationElement =
                            configurationReader.Current as OAuthConfigurationElement;

                        if (oConfigurationName != null)
                        {
                            if (clientConfigurationElement.Name == oConfigurationName)
                            {
                                var client = (T) Activator.CreateInstance(typeof (T), new object[]
                                {
                                    clientConfigurationElement.ClientId,
                                    clientConfigurationElement.ClientSecret,
                                    clientConfigurationElement.CallbackUrl,
                                    clientConfigurationElement.Scope,
                                    ccWebElem.AcceptedRedirectUrl,
                                    ccWebElem.FailedRedirectUrl,
                                    ccWebElem.Proxy
                                });

                                return client;
                            }
                        }
                        else
                        {
                            throw new Exception("ERROR: [MultiOAuthFactroy] ConfigurationName is not found!");
                        }
                    }
                }
            }

            return default(T);
        }

        //public static T CreateClient<T>(string oClientId, string oClientSecret, string oCallbackUrl, string oScope,
        //    string oAcceptedUrl, string oFailedUrl, string oProxy) where T : AbstractClientProvider, new()
        //{
        //    var client = (T) Activator.CreateInstance(typeof (T), new object[]
        //    {
        //        oClientId,
        //        oClientSecret,
        //        oCallbackUrl,
        //        oScope,
        //        oAcceptedUrl,
        //        oFailedUrl,
        //        oProxy
        //    });
        //    return client;
        //}
    }
}
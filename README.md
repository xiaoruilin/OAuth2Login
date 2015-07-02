# Oauth2Login library
	
ASP.NET C# Opensource library that abstracts social logins for providers like Facebook, Google, Twitter, PayPal.

Detailed article explaining how to use the library can be found on CodeProject:
[OAuth2 Social logins open source library - Facebook, Google, Twitter, PayPal](http://www.codeproject.com/Articles/1006013/OAuth-Social-logins-open-source-library-Facebook-G)

## Quick start

After cloning repository you need to create Web.Oauth.config file and populate it with ClientId & ClientSecret values tied to your apps. Here is example config file:

    <?xml version="1.0"?>
    <oauth2.login.configuration>
      <web failedRedirectUrl="~/AuthExternal/LoginFail" />
      <oauth>
        <add name="Google"
             clientid="xxx_CLIENT_ID_xxx"
             clientsecret="xxx_CLIENT_SECRET_xxx"
             callbackUrl="http://localhost:28950/AuthExternal/Callback/Google"
             scope="https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile" />
    
        <add name="Facebook"
             clientid="xxx_CLIENT_ID_xxx"
             clientsecret="xxx_CLIENT_SECRET_xxx"
             callbackUrl="http://localhost:28950/AuthExternal/Callback/Facebook"
             scope="public_profile,user_friends,email" />
        
        <add name="PayPal"
             clientid="xxx_CLIENT_ID_xxx"
             clientsecret="xxx_CLIENT_SECRET_xxx"
             callbackUrl="http://localhost:28950/AuthExternal/Callback/PayPal"
             scope="openid email"
             endpoint="sandbox" />
    
        <add name="Twitter"
             clientid="xxx_CLIENT_ID_xxx"
             clientsecret="xxx_CLIENT_SECRET_xxx"
             callbackUrl="http://127.0.0.1:28950/AuthExternal/Callback/Twitter" />
      </oauth>
    </oauth2.login.configuration>


## License

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
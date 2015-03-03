using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    public abstract class BaseUserData
    {
        protected BaseUserData(ExternalAuthServices authService)
        {
            AuthService = authService;
        }

        public abstract string UserId { get; set; }
        public abstract string Email { get; set; }
        public abstract string PhoneNumber { get; set; }
        
        public ExternalAuthServices AuthService { get; private set; }
    }
}

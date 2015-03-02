using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oauth2Login.Core;

namespace MultipleOauth2Mvc.Controllers
{
    public class AuthExternalController : Controller
    {
        // TODO: REFACTOR THIS - static reference is bad - it'll only work for one login session at the time
        private static Oauth2LoginContext _context;
        public ActionResult Login(string id)
        {
            var client = AbstractClientProvider.ClientById(id);

            if (client != null)
            {
                //var context = Oauth2LoginContext.Create(client);
                _context = Oauth2LoginContext.Create(client);
                var url = _context.BeginAuth();

                return Redirect(url);
            }
            else
            {
                return RedirectToAction("LoginFail");
            }
        }

        public ActionResult Callback(string id)
        {
            var client = AbstractClientProvider.ClientById(id);

            if (client != null)
            {
                try
                {
                    //var context = Oauth2LoginContext.Create(client);

                    var redirectUrl = _context.ValidateLogin();
                    if (redirectUrl != null)
                    {
                        return Redirect(redirectUrl);
                    }

                    var token = _context.Token;
                    var result = _context.Profile;
                    var strResult = _context.Client.ProfileJsonString;

                    return View(new AuthCallbackResult
                    {
                        RedirectUrl = "/AuthExternal/LoginSuccess"
                    });
                }
                catch (Exception)
                {
                    throw;
                    //RedirectToAction("Error");
                }
            }
            else
            {
                return RedirectToAction("LoginFail");
            }
        }

        public ActionResult LoginFail()
        {
            return View();
        }

        public ActionResult LoginSuccess()
        {
            return View();
        }

        // plumbing
    }

    public class AuthCallbackResult
    {
        public string ErrorText { get; set; }

        public string RedirectUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oauth2Login.Core;
using Oauth2Login.Service;

namespace MultipleOauth2Mvc.Controllers
{
    public class AuthExternalController : Controller
    {
        public ActionResult Login(string id)
        {
            var service = BaseOauth2Service.GetService(id);

            if (service != null)
            {
                var url = service.BeginAuthentication();

                return Redirect(url);
            }
            else
            {
                return RedirectToAction("LoginFail");
            }
        }

        public ActionResult Success(string id)
        {
            var service = BaseOauth2Service.GetService(id);

            if (service != null)
            {
                try
                {
                    var redirectUrl = service.ValidateLogin(Request);
                    if (redirectUrl != null)
                    {
                        return Redirect(redirectUrl);
                    }

                    // data in service._client now... think on how to expose it
                    //var token = _context.Token;
                    //var result = _context.Profile;
                    //var strResult = _context.Client.ProfileJsonString;

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
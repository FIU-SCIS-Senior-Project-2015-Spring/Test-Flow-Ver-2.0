using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestFlow.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            HttpCookie cookie = base.Request.Cookies["TSWA-Last-User"];

            if (base.User.Identity.IsAuthenticated == false || cookie == null || StringComparer.OrdinalIgnoreCase.Equals(base.User.Identity.Name, cookie.Value))
            {

                string name = string.Empty;
                if (base.Request.IsAuthenticated)
                {
                    name = this.User.Identity.Name;
                }

                cookie = new HttpCookie("TSWA-Last-User", name);
                base.Response.Cookies.Set(cookie);

                base.Response.AppendHeader("Connection", "close");
                base.Response.StatusCode = 0x191;
                base.Response.Clear();
                //should probably do a redirect here to the unauthorized/failed login page
                //if you know how to do this, please tap it on the comments below
                base.Response.Write("PageResources.UnauthorizedAccessMessage");
                base.Response.End();
                return RedirectToAction("Index");
            }

            cookie = new HttpCookie("TSWA-Last-User", string.Empty)
            {
                Expires = DateTime.Now.AddYears(-5)
            };
            base.Response.Cookies.Set(cookie);

            return RedirectToAction("Index");
        }
    }
}
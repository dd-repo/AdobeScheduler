using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AdobeScheduler.Security
{
    public class AdobeAuthorizeAttribute: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (HttpContext.Current.Session["AdobeObj"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"action","Login"},
                            {"controller","Auth"}
                        });

            }
        }
    }
}
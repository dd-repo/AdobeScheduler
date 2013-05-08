using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AdobeScheduler.Security
{
    public class AdobeAuthorizeAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (HttpContext.Current.Session["UserSession"] == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
               {
                   controller = "Auth",
                   action = "Login"
               }));
            }
        }
    }
}
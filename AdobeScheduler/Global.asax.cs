using AdobeScheduler.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace AdobeScheduler
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static RouteBase hubRoute;
       
        protected void Application_Start(object sender, EventArgs e)
        {
            hubRoute = RouteTable.Routes.MapHubs();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (IsSignalRRequest(Context))
            {
                Context.SetSessionStateBehavior(SessionStateBehavior.ReadOnly);
            }
        }

        private bool IsSignalRRequest(HttpContext context)
        {
            RouteData routeData = hubRoute.GetRouteData(new HttpContextWrapper(context));
            return routeData != null;
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null || authTicket.Expired)
                {
                    return;
                }

                Identity id = new Identity(authTicket.Name, authTicket.UserData);
                GenericPrincipal user = new GenericPrincipal(id, null);
                Context.User = user;
                Thread.CurrentPrincipal = user;
            }
        }

    }
}
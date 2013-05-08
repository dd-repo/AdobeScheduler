using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdobeScheduler.Security;
using AdobeConnectSDK;
using AdobeScheduler.Models;

namespace AdobeScheduler.Controllers
{


    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        [Authorize]
        [AdobeAuthorize]
        public ActionResult Index()
        {
            UserSession model = (UserSession)Session["UserSession"];
            ViewObject viewObject = new ViewObject(model);
            return View(viewObject);
        }

        [Authorize]
        [AdobeAuthorize]
        public ActionResult Report()
        {
            return View();
        }

        [Authorize]
        public ActionResult Room(string room)
        {
            UserSession model = (UserSession)Session["UserSession"];

            using (AdobeConnectDB _db = new AdobeConnectDB())
            {
                room = "/" + room + "/";
                var query = (from r in _db.Appointments where r.url == room select r).First();
                if (query != null)
                {
                    ViewObject viewObject = new ViewObject(model, query);
                    return View(viewObject);
                }
                else
                {
                    ViewBag.Course = room;
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            
        }

        


    }
}

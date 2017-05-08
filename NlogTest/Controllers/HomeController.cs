using NLog;
using NlogTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NlogTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                Log.Instance.Debug("We're going to throw an exception now.");
                Log.Instance.Warn("It's gonna happen!!");
                throw new ApplicationException();
            }
            catch (ApplicationException ae)
            {
                Log.Instance.ErrorException("Error doing something...", ae);
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
using IndividualLogins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndividualLogins.Controllers
{
    public class CarsController : Controller
    {
        // GET: Cars
        public ActionResult Index()
        {
            using (RatesDBContext ctx = new RatesDBContext())
            {
                return View(ctx.Cars.Where(c => c.IsAssigned).OrderBy(o => o.Category).ToList());
            }
        }
    }
}
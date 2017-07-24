using IndividualLogins.Controllers.App_Code;
using IndividualLogins.Models;
using IndividualLogins.Models.Dal;
using IndividualLogins.Models.NlogTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndividualLogins.Controllers
{
    public class HomeController : Controller
    {
        PricingToolDal dal = new PricingToolDal();
        [Authorize(Roles = "Admin, Edit, Preview")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Edit") || User.IsInRole("Preview"))
            {
                ViewBag.Locations = dal.GetLocations();
                ViewBag.Sources = dal.GetSources();
                return View(new SearchFilters());
            }
            else
                return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Admin, Edit, Preview")]
        public string GetResultFileName(SearchFilters searchFilters)
        {
            Log.Instance.Warn("---Begin: GetResultFileName");
            string fileName = "";
            SiteBase site = null;
            try
            {
                if (ModelState.IsValid)
                {
                    Log.Instance.Warn("---: GetResultFileName");
                    DbUpdates.PdfCreated(searchFilters, User.Identity.Name);

                    return fileName = new Rates().GetPdfLocation(site, searchFilters);
                }
                else
                {
                    Log.Instance.Warn("---else: GetResultFileName");

                    return "";
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error("--- " + ex.Message + "\n " + ex.InnerException + "\n" + ex.StackTrace);
                return "";
            }
        }

        [Authorize(Roles = "Admin, Edit, Preview")]
        public ActionResult News()
        {
            using (RatesDBContext ctx = new RatesDBContext())
            {
                return View(ctx.News.ToList());
            }
        }

        public JsonResult GetLocations(int? country)
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            switch (country)
            {
                case 1:
                    sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
                    break;
                case 2:
                    sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
                    break;
                case 3:
                    sl.Add(new SelectListItem { Selected = false, Text = "Gdansk", Value = "12" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Krakow", Value = "11" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Chopin)", Value = "4" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Modlin)", Value = "9" });
                    break;
                case 4:
                    sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
                    break;
                case 5:
                    sl.Add(new SelectListItem { Selected = false, Text = "Fiumicino", Value = "6" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Rome", Value = "7" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Bologna", Value = "8" });
                    break;
                case 6:
                    sl.Add(new SelectListItem { Selected = false, Text = "Prague", Value = "10" });
                    break;
                default:
                    break;
            }
            IEnumerable<SelectListItem> data = new SelectList(sl, "Value", "Text");

            return new JsonResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
using IndividualLogins.Models;
using IndividualLogins.Controllers.App_Code;
using IndividualLogins.Models.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace IndividualLogins.Controllers
{
    [Authorize]
    public class PricingController : Controller
    {
        List<PricingClass> allClasses;

        public PricingController()
        {
            allClasses = AllClasses();
        }
        // GET: Pricing
        public ActionResult Index()
        {
            ViewBag.Locations = GetLocations();
            ViewBag.Sources = new PricingToolDal().GetSources();
            PricingModel pm = new PricingModel();
            pm.AvailableClasses = GetClasses();
            return View(pm);
        }

        public IEnumerable<SelectListItem> GetLocations()
        {

            //using (Rates1 ctx = new Rates1())
            {
                List<SelectListItem> sl = new List<SelectListItem>();
                sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Modlin)", Value = "9" });
                //foreach (Models.Location l in ctx.Locations1)
                //{
                //    sl.Add(new SelectListItem { Selected = false, Text = l.Name, Value = l.Id.ToString() });
                //}
                return new SelectList(sl, "Value", "Text");
            }
                
            //sl.Add(new SelectListItem { Selected = false, Text = "Gdansk", Value = "12" });
            //sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Krakow", Value = "11" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Chopin)", Value = "4" });
            
            //sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Fiumicino", Value = "6" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Rome", Value = "7" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Bologna", Value = "8" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Prague", Value = "10" });


            
        }
        public string GeneratePrices(PricingModel searchFilters)
        {
            if (ModelState.IsValid)
            {
                PricingHelper pr = new PricingHelper(searchFilters,
                allClasses.Where(s => s.LocationId == searchFilters.Location && s.IntervalNum == searchFilters.IntervalNum && searchFilters.Classes.FirstOrDefault().Contains(s.ClassName)).ToArray());
                return pr.Excecute();
            }
            else
                return "";
        }

        public string InitiateDates(PricingModel searchFilters)
        {
            if (ModelState.IsValid)
            {
                PricingHelper pr = new PricingHelper(searchFilters,
                allClasses.Where(s => s.LocationId == searchFilters.Location && s.IntervalNum == searchFilters.IntervalNum && searchFilters.Classes.FirstOrDefault().Contains(s.ClassName)).ToArray());

                return pr.SetDates();
            }
            else
                return "";
        }


        private List<SelectListItem> GetClasses()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (string s in AllClasses().Select(s => s.ClassName).Distinct())
            {
                selectList.Add(new SelectListItem { Text = s, Value = s });
            }
            return selectList;
        }

        private List<PricingClass> AllClasses()
        {
            allClasses = new List<PricingClass>();
            allClasses.Add(new PricingClass("MCMR", "MiniM", "296333", 9, 1));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "296335", 9, 1));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "296337", 9, 1));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "307349", 9, 1));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "307352", 9, 1));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "296332", 9, 2));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "296334", 9, 2));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "296336", 9, 2));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "307350", 9, 2));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "307353", 9, 2));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "295233", 9, 3));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "295235", 9, 3));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "295234", 9, 3));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "307351", 9, 3));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "307355", 9, 3));

            return allClasses;
        }
    }
}
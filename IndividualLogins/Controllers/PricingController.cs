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
        PricingToolDal dal = new PricingToolDal();

        public PricingController()
        {
            allClasses = AllClasses();
        }
        // GET: Pricing
        [Authorize(Roles = "Admin, Edit")]
        public ActionResult Index()
        {
            ViewBag.Locations = dal.GetLocations();
            ViewBag.Sources = dal.GetSources();
            PricingModel pm = new PricingModel();
            pm.AvailableClasses = GetClasses();
            return View(pm);
        }

        [Authorize(Roles = "Admin, Edit")]
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

        [Authorize(Roles = "Admin, Edit")]
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

            foreach (PricingClass s in AllClasses().Take(5))
            {
                selectList.Add(new SelectListItem { Text = s.PublicName, Value = s.ClassName });
            }
            return selectList;
        }

        private List<PricingClass> AllClasses()
        {
            allClasses = new List<PricingClass>();
            //--Modlin 1
            allClasses.Add(new PricingClass("MCMR", "MiniM", "296333", 9, 1));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "296335", 9, 1));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "296337", 9, 1));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "307349", 9, 1));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "307352", 9, 1));
            //--Modlin 2
            allClasses.Add(new PricingClass("MCMR", "MiniM", "296332", 9, 2));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "296334", 9, 2));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "296336", 9, 2));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "307350", 9, 2));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "307353", 9, 2));
            //--Modlin 3
            allClasses.Add(new PricingClass("MCMR", "MiniM", "295233", 9, 3));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "295235", 9, 3));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "295234", 9, 3));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "307351", 9, 3));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "307355", 9, 3));

            //--Modlin 4
            allClasses.Add(new PricingClass("MCMR", "MiniM", "311917", 9, 4));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "311958", 9, 4));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311952", 9, 4));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311960", 9, 4));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311964", 9, 4));

            //--Modlin 5
            allClasses.Add(new PricingClass("MCMR", "MiniM", "311930", 9, 5));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "311959", 9, 5));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311953", 9, 5));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311962", 9, 5));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311965", 9, 5));

            //--Modlin 5
            allClasses.Add(new PricingClass("MCMR", "MiniM", "311970", 9, 6));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "311960", 9, 6));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311957", 9, 6));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311963", 9, 6));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311966", 9, 6));

            //--VIlnius 1
            allClasses.Add(new PricingClass("MCMR", "MiniM", "310881", 1, 1));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "310891", 1, 1));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311206", 1, 1));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311226", 1, 1));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311236", 1, 1));
            ////--Vilnius 2
            allClasses.Add(new PricingClass("MCMR", "MiniM", "310886", 1, 2));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "310896", 1, 2));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311211", 1, 2));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311231", 1, 2));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311241", 1, 2));
            ////--Vilnisu 3
            allClasses.Add(new PricingClass("MCMR", "MiniM", "311196", 1, 3));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "311201", 1, 3));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311216", 1, 3));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311251", 1, 3));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311246", 1, 3));


            return allClasses;
        }
    }
}
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
using IndividualLogins.Models.NlogTest.Models;

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
        
        [Authorize(Roles = "Admin, Edit")]
        public ActionResult Index()
        {
            ViewBag.Locations = dal.GetLocations();
            ViewBag.Sources = dal.GetSources();
            PricingModel pm = new PricingModel();
            pm.AvailableClasses = GetClasses();
            return View(pm);
        }

        public JsonResult GetIntervals(int? locationId)
        {
            IntervalDates[] intDates = new IntervalDates[7];
            if (locationId > 0)
            {
                using (RatesDBContext ctx = new RatesDBContext())
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        Update updt = ctx.Updates.Where(w => w.LocationId == locationId && w.IntervalNum == i).OrderByDescending(o => o.UpdateTime).FirstOrDefault();
                        if (updt != null) 
                            intDates[i] = new IntervalDates {PuDate = updt.PickupTime, DoDate = updt.DropoffTime };
                    }
                }
            }

            return new JsonResult()
            {
                Data = intDates,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        [Authorize(Roles = "Admin, Edit")]
        public string GeneratePrices(PricingModel searchFilters)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DbUpdates.UpdatedRates(searchFilters, User.Identity.Name);

                    PricingHelper pr = new PricingHelper(searchFilters,
                    allClasses.Where(s => s.LocationId == searchFilters.Location && s.IntervalNum == searchFilters.IntervalNum && searchFilters.Classes.FirstOrDefault().Contains(s.ClassName)).ToArray());
                    return pr.Excecute();
                }
                else
                    return "";
            }
            catch(Exception ex)
            {
                Log.Instance.Error("--- " + ex.Message + "\n " + ex.InnerException + "\n" + ex.StackTrace);
                return "";
            }
        }

        [Authorize(Roles = "Admin, Edit")]
        public string InitiateDates(PricingModel searchFilters)
        {
            if (ModelState.IsValid)
            {
                DbUpdates.UpdatedRates(searchFilters, User.Identity.Name);

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
            allClasses.Add(new PricingClass("MCMR", "MiniM", "351453", 1, 1));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "351458", 1, 1));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "351463", 1, 1));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "351488", 1, 1));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "351498", 1, 1));
            ////--Vilnius 2
            allClasses.Add(new PricingClass("MCMR", "MiniM", "351478", 1, 2));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "351473", 1, 2));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "351468", 1, 2));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "351483", 1, 2));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "351493", 1, 2));
            ////--Vilnisu 3
            allClasses.Add(new PricingClass("MCMR", "MiniM", "311196", 1, 3));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "311201", 1, 3));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "311216", 1, 3));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "311251", 1, 3));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "311246", 1, 3));

            //--VIlnius 4
            allClasses.Add(new PricingClass("MCMR", "MiniM", "312102", 1, 4));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "312117", 1, 4));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "312137", 1, 4));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "312152", 1, 4));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "312167", 1, 4));
            ////--Vilnius 5
            allClasses.Add(new PricingClass("MCMR", "MiniM", "312107", 1, 5));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "312122", 1, 5));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "312142", 1, 5));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "312157", 1, 5));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "312172", 1, 5));
            ////--Vilniss 6
            allClasses.Add(new PricingClass("MCMR", "MiniM", "312112", 1, 6));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "312127", 1, 6));
            allClasses.Add(new PricingClass("EDAR", "EconomyA", "312147", 1, 6));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "312162", 1, 6));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "312177", 1, 6));

            ////--Riga mini
            allClasses.Add(new PricingClass("EDMR", "MiniM", "314141", 3, 1));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "314146", 3, 2));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "314151", 3, 3));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "314156", 3, 4));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "314161", 3, 5));
            allClasses.Add(new PricingClass("MCMR", "MiniM", "314166", 3, 6));

            ////--Riga ekom
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "314171", 3, 1));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "314176", 3, 2));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "314181", 3, 3));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "314186", 3, 4));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "314191", 3, 5));
            allClasses.Add(new PricingClass("EDMR", "EconomyM", "314196", 3, 6));

            ////--Riga compactm
            allClasses.Add(new PricingClass("CDMR", "CompactM", "314201", 3, 1));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "314206", 3, 2));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "314211", 3, 3));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "314216", 3, 4));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "314251", 3, 5));
            allClasses.Add(new PricingClass("CDMR", "CompactM", "314256", 3, 6));

            ////--Riga compacta
            allClasses.Add(new PricingClass("CDAR", "CompactA", "314221", 3, 1));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "314226", 3, 2));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "314231", 3, 3));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "314236", 3, 4));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "314241", 3, 5));
            allClasses.Add(new PricingClass("CDAR", "CompactA", "314246", 3, 6));

            return allClasses;
        }
    }
}
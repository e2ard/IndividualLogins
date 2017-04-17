using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace IndividualLogins.Models.Dal
{
    public class PricingToolDal
    {
        public SelectList GetSources()
        {
            return new SelectList(new List<SelectListItem>{
                  new SelectListItem{ Selected = false,Text = "RentalCars", Value = "1"},
                  new SelectListItem {Selected = false, Text = "CarsTrawler", Value = "2"},
                  new SelectListItem {Selected = false, Text = "CarScanner", Value = "3"},
                  //new SelectListItem {Selected = false, Text = "EcoBookings", Value = "4"},
                  //new SelectListItem {Selected = false, Text = "Expedia", Value = "5"}
            }, "Value", "Text");
        }

        public SelectList GetCoutries()
        {
            return new SelectList(new List<SelectListItem>{
                  new SelectListItem{ Selected = false,Text = "Latvia", Value = "1"},
                  new SelectListItem {Selected = false, Text = "Lithuania", Value = "2"},
                  new SelectListItem{ Selected = false,Text = "Poland", Value = "3"},
                  //new SelectListItem{ Selected = false,Text = "UK", Value = "4"},
                  //new SelectListItem{ Selected = false,Text = "Italy", Value = "5"},
                  //new SelectListItem{ Selected = false,Text = "Czech", Value = "6"}
            }, "Value", "Text");
        }

        public SelectList GetLocations()
        {
            return new SelectList(new List<SelectListItem>(), "Value", "Text");
        }

        //public static JsonResult GetLocations(int country)
        //{
        //    List<SelectListItem> sl = new List<SelectListItem>();
        //    switch (country)
        //    {
        //        case 1:
        //            sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
        //            break;
        //        case 2:
        //            sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
        //            sl.Add(new SelectListItem { Selected = false, Text = "Kaunasa", Value = "2" });
        //            break;
        //        case 3:
        //            sl.Add(new SelectListItem { Selected = false, Text = "Warsawa", Value = "4" });
        //            break;
        //        case 4:
        //            sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
        //            break;
        //        default:
        //            break;
        //    }
        //    return new JsonResult()
        //    {
        //        Data = new SelectList(sl, "Value", "Text"),
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}
    }
}
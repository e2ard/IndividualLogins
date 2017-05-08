using IndividualLogins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndividualLogins.Models
{
    public class PricingModel : SearchFilters
    {
        public List<string> Classes { get; set;  }
        public List<SelectListItem> AvailableClasses { get; set; }
        public bool ApplyToAll { get; set; }

        public string GetString()
        {
            return "loc:" + Location + " intvNr:" + IntervalNum + " cls:" + Classes;
        }
    }

    public class PricingClass
    {
        public string ClassName { get; set; }
        public string ClassLinkId { get; set; }

        public int LocationId { get; set; }

        public int IntervalNum { get; set; }

        public string PublicName { get; set; }

        public PricingClass(string publicName, string name, string linkId, int locationId, int intervalNum)
        {
            PublicName = publicName;
            ClassName = name;
            ClassLinkId = linkId;
            LocationId = locationId;
            IntervalNum = intervalNum;
        }
    }

    public class IntervalResults
    {
        public int[] Intervals { get; set; }
        public IntervalDates[] Dates { get; set; }
    }

    public class IntervalDates
    {
        public DateTime? PuDate { get; set; }
        public DateTime? DoDate { get; set; }
    }
}
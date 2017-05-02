using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndividualLogins.Models
{
    public static class DbUpdates
    {
        public static void UpdatedRates(PricingModel searchFilters, string userName)
        {
            using (RatesDBContext ctx = new RatesDBContext())
            {
                ctx.Updates.Add(new Update
                {
                    IntervalNum = searchFilters.IntervalNum,
                    LocationId = searchFilters.Location,
                    UpdateTime = DateTime.Now,
                    Username = userName,
                    Params = "src:" + searchFilters.Source + " cls: "  +string.Join(",", searchFilters.Classes),
                    PickupTime = searchFilters.PuDate,
                    DropoffTime = searchFilters.DoDate
                });
                ctx.SaveChanges();
            }
        }

        public static void PdfCreated(SearchFilters searchFilters, string userName)
        {
            using (RatesDBContext ctx = new RatesDBContext())
            {
                ctx.Updates.Add(new Update
                {
                    IntervalNum = 0,
                    LocationId = searchFilters.Location,
                    UpdateTime = DateTime.Now,
                    Username = userName,
                    Params = "src:" + searchFilters.Source,
                    PickupTime = searchFilters.PuDate,
                    DropoffTime = searchFilters.DoDate
                });
                ctx.SaveChanges();
            }
        }

    }
}
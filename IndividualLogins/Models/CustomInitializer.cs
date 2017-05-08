using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IndividualLogins.Models
{
    public class CustomInitializer : CreateDatabaseIfNotExists<RatesDBContext>
    {
        protected override void Seed(RatesDBContext context)
        {
            IList<Location> defaultStandards = new List<Location>();

            defaultStandards.Add(new Location { LocationId = 1, Name = "Vilnius", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 9, Name = "Warsaw (Modlin)", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 4, Name = "Warsaw (Chopin)", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 3, Name = "Riga", IsAvailable = true });

            foreach (Location std in defaultStandards)
                context.Locations.Add(std);

            base.Seed(context);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IndividualLogins.Models
{
    public class CustomInitializer : DropCreateDatabaseIfModelChanges<RatesDBContext>
    {
        protected override void Seed(RatesDBContext context)
        {
            IList<Location> defaultStandards = new List<Location>();

            defaultStandards.Add(new Location { LocationId = 1, Name = "Vilnius", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 9, Name = "Warsaw (Modlin)", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 4, Name = "Warsaw (Chopin)", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 3, Name = "Riga", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 2, Name = "Kaunas", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 11, Name = "Krakow", IsAvailable = true });
            defaultStandards.Add(new Location { LocationId = 12, Name = "Gdansk", IsAvailable = true });

            foreach (Location std in defaultStandards)
                context.Locations.Add(std);

            base.Seed(context);
        }
    }
}
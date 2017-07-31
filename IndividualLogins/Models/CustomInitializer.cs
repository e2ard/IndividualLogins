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
            FillLocations(context);
            FillNews(context);

            base.Seed(context);
        }

        private void FillLocations(RatesDBContext context)
        {
            IList<Location> locations = new List<Location>();

            locations.Add(new Location { LocationId = 1, Name = "Vilnius", IsAvailable = true });
            locations.Add(new Location { LocationId = 9, Name = "Warsaw (Modlin)", IsAvailable = true });
            locations.Add(new Location { LocationId = 4, Name = "Warsaw (Chopin)", IsAvailable = true });
            locations.Add(new Location { LocationId = 3, Name = "Riga", IsAvailable = true });
            locations.Add(new Location { LocationId = 2, Name = "Kaunas", IsAvailable = true });
            locations.Add(new Location { LocationId = 11, Name = "Krakow", IsAvailable = true });
            locations.Add(new Location { LocationId = 12, Name = "Gdansk", IsAvailable = true });

            context.Locations.AddRange(locations);

        }
        private void FillNews(RatesDBContext ctx)
        {
            IList<News> news = new List<News>();
            news.Add(new News{ UpdateDate = new DateTime(2017, 7, 25), Text = "Excel feature" });
            news.Add(new News { UpdateDate = new DateTime(2017, 7, 30), Text = "Excel update" });

            ctx.News.AddRange(news);
        }
    }
}
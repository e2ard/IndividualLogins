using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IndividualLogins.Models
{
    public class RatesDBContext : DbContext
    {
        public RatesDBContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RatesDBContext>(new DropCreateDatabaseIfModelChanges<RatesDBContext>());
        }

        public static RatesDBContext Create()
        {
            return new RatesDBContext();
        }
        public DbSet<Update> Updates { get; set; }
    }

    public class Update
    {
        public int UpdateId { get; set; }
        public int IntervalNum { get; set; }
        public int LocationId { get; set; }
        public string Username { get; set; }
        public DateTime UpdateTime { get; set; }

        public DateTime PickupTime { get; set; }

        public DateTime DropoffTime { get; set; }

        public string Params { get; set; }

    }
}
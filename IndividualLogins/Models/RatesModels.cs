using System;
using System.Data.Entity;

namespace IndividualLogins.Models
{
    public class RatesDBContext : DbContext
    {
        public RatesDBContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RatesDBContext>(new CustomInitializer());
        }

        public static RatesDBContext Create()
        {
            return new RatesDBContext();
        }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<News> News { get; set; }

        public DbSet<Cars> Cars { get; set; }

        public DbSet<PricingInterval> Classes { get; set; }
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

    public class Location
    {
        public int LocationId { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class News
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime UpdateDate { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class Cars
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public float Price { get; set; }
        public string Transmission { get; set; }
        public float Seats { get; set; }
        public string CarName { get; set; }
        public string Category { get; set; }
        public int SupplierType { get; set; }

        public bool IsAssigned { get; set; }
    }

    public class PricingInterval
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string ClassLinkId { get; set; }

        public int LocationId { get; set; }

        public int IntervalNum { get; set; }

        public string CategoryName { get; set; }
        
    }
}
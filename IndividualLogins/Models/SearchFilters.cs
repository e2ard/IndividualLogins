using IndividualLogins.Models.Dal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace IndividualLogins.Models
{
    public class SearchFilters:IValidatableObject
    {
        public int IntervalNum { get; set; }

        [Required(ErrorMessage = "Source not chosen")]
        [Range(1, 100)]
        public int? Source { get; set; }
        public IEnumerable<SelectListItem> Sources { get; }
        //[Required(ErrorMessage = "Country not chosen")]
        //[Range(1, 100, ErrorMessage = "Please choose a country")]
        //public int? Country { get; set; }
        public IEnumerable<SelectListItem> Countries { get; }
        [Range(1, 100, ErrorMessage = "Please choose a location")]
        [Required(ErrorMessage = "Location not chosen")]
        public int Location { get; set; }
        public IEnumerable<SelectListItem> Locations{ get; }
        [Required(ErrorMessage = "Drop-off date not chosen")]
        public DateTime DoDate { get; set; }
        public TimeSpan DoTime { get; set; }
        [Required(ErrorMessage = "Pick-up date not chosen")]
        public DateTime PuDate { get; set; }
        public TimeSpan PuTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            PuDate = PuDate.Add(PuTime);
            DoDate = DoDate.Add(DoTime);

            if ((DoDate - PuDate).Days > 30)
                results.Add(new ValidationResult("Date interval cannot be more than 30 days" + DoDate + " " + PuDate + " " + (DoDate - PuDate).Days, new string[] { "DoDate" }));

            if (DoDate < PuDate)
                results.Add(new ValidationResult("Drop off date is less than pick up date ", new string[] { "DoDate" }));

            if (PuDate == DoDate)
                results.Add(new ValidationResult("Date interval cannot be less than 1 day", new string[] { "DoDate" }));

            if (PuDate.AddHours(-2) <= DateTime.Now)
                results.Add(new ValidationResult("Pick-up date cannot be in the past or earlier than 2 hours to reservation", new string[] { "PuDate" }));

            if (DoDate.AddHours(-2) <= DateTime.Now)
                results.Add(new ValidationResult("Drop-off date cannot be in the past or earlier than 2 hours to reservation", new string[] { "DoDate" }));
            
            return results;
        }
    }
}
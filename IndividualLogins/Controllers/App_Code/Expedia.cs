using IndividualLogins.Controllers.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IndividualLogins.Controllers.App_Code
{
    public class Expedia : SiteBase
    {
        public Expedia(string site1)
        {
            SetSiteName(site1);
        }

        public override string GetCity()
        {
            return "Fiumi";
        }

        public override string GetPuDay()
        {
            return "12";
        }

        public override string GetPuMonth()
        {
            return "12";
        }

        public override string GetPuYear()
        {
            return "1212";
        }

        public override void InitDate(DateTime date)// set pu date & do date = pu date + 1
        {
            string dateToSearch = "pickUpDate=\\d\\d%2F\\d\\d%2F201\\d";
            string dateToChange = "pickUpDate="+ AddZero(date.Day) + "%2F" + AddZero(date.Month) + "%2F"+ date.Year;
            Regex rgx = new Regex(dateToSearch);
            siteName = rgx.Replace(siteName, dateToChange);

            dateToSearch = "dropOffDate=\\d\\d%2F\\d\\d%2F201\\d";
            dateToChange = "dropOffDate=" + AddZero(date.Day) + "%2F" + AddZero(date.Month) + "%2F" + date.Year;

            rgx = new Regex(dateToSearch);
            SetSiteName(rgx.Replace(siteName, dateToChange));
        }

        public override void SetDoDay(DateTime date)// set do date
        {
            string dateToChange = "dropOffDate=\\d\\d%2F\\d\\d%2F201\\d";
            string replacement = "dropOffDate=" + AddZero(date.Day) + "%2F" + AddZero(date.Month) + "%2F" + date.Year;

            Regex rgx = new Regex(dateToChange);
            siteName = rgx.Replace(siteName, replacement);
        }

        public override string GetTitle()
        {
            return "Expedia";
        }
    }
}
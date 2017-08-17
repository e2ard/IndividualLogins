using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace IndividualLogins.Controllers.App_Code
{
    public class Trawler : SiteBase
    {
        readonly private string PuDatePattern = "PickUpDateTime%22:%22201(\\d)-\\d+-\\d+T\\d+:\\d+";
        readonly private string DoDatePattern = "ReturnDateTime%22:%22201\\d-\\d+-\\d+T\\d+:\\d+";
        public Trawler(string site1)
        {
            SetSiteName(site1);
            SetTitle(ParseTitle(site1));
        }

        public override void InitDate(DateTime date)
        {
            //string dateToChange = "ReturnDateTime%22:%22" + date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day) + "T" + AddZero(date.Hour) + ":" + AddZero(date.Minute);

            string dateToChange = "ReturnDateTime%22:%22" + GetDateString(date, ":");

            Regex rgx = new Regex(DoDatePattern);
            siteName = rgx.Replace(siteName, dateToChange);

            dateToChange = "PickUpDateTime%22:%22" + GetDateString(date, ":");//+ date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day) + "T" + AddZero(date.Hour) + ":" + AddZero(date.Minute);

            rgx = new Regex(PuDatePattern);
            SetSiteName(rgx.Replace(siteName, dateToChange));
        }

        public override void SetDoDay(DateTime date)
        {
            string replacement = "ReturnDateTime%22:%22" + GetDateString(date, ":");// +date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day) + "T" + AddZero(date.Hour) + ":" + AddZero(date.Minute);
            //to do common method

            Regex rgx = new Regex(DoDatePattern);
            siteName = rgx.Replace(siteName, replacement);
        }

        public String GetLocation()
        {
            string pattern = "LocationCode%22:%22";
            Match matchDetails = Regex.Match(GetSiteName(), pattern + "\\d*");
            if (matchDetails.Captures.Count == 0)
            {
                pattern = "LocationCode%22%3A%22";
                matchDetails = Regex.Match(GetSiteName(), pattern + "\\d*");
            }
            if (matchDetails.Captures.Count == 0)
            {
                pattern = "LocationCode%22:";
                matchDetails = Regex.Match(GetSiteName(), pattern + "\\d*");
            }
            string temp = matchDetails.Captures[0].Value;
            return temp.Substring(pattern.Length, temp.Length - pattern.Length);
        }

        private String GetCityByCode(string locationCode)
        {
            string location = string.Empty;
            switch(locationCode)
            {
                case "3224":
                    return "Vilnius";
                case "4674":
                    return "Kaunas";
                case "3204":
                case "150965":
                    return "Warsaw";
                case "4305":
                    return "Riga";
                case "3197":
                    return "Krakow";
                case "3195":
                    return "Gdansk";
                case "4220":
                    return "Katowice";
            }
            return "Unknown";
        }

        public override string GetCity()
        {
            try
            {
                return GetCityByCode(GetLocation());
            }
            catch
            {
                return "Unknown"; //TOFIX 
            }
        }

        public override string GetPuDay()
        {
            Match matchDetails = Regex.Match(GetSiteName(), PuDatePattern);
            string temp = matchDetails.Captures[0].Value;
            return temp.Substring(temp.Length - 8, 2);
        }

        public override string GetPuMonth()
        {
            Match matchDetails = Regex.Match(GetSiteName(), PuDatePattern);
            string temp = matchDetails.Captures[0].Value;
            return temp.Substring(temp.Length - 11, 2);
        }

        public override string ParseTitle(string siteName)
        {
            string pattern = ".*(\\.).{3}\\/";
            Match matchDetails = Regex.Match(GetSiteName(), pattern);

            string title = matchDetails.Captures[0].Value.Substring(pattern.Length - 5, matchDetails.Captures[0].Value.Length - (pattern.Length - 5));

            return title.Substring(8, title.Length - 13);
        }

        public override string GetPuYear()
        {
            return "2016";
        }
    }
}
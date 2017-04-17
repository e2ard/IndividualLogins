using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndividualLogins.Controllers.App_Code
{
    public class Rental : SiteBase
    {
        public Rental(string site1)
        {
            this.siteName = site1;
        }

        public string AddTransm(string transmission)
        {
            const string filterTransm = "&filterTransmission=";
            if (!this.siteName.Contains(transmission))
            {
                return string.Concat(this.siteName, filterTransm + transmission);
            }
            else
            {
                return this.siteName;
            }
        }

        public List<string> GetGeneratedLinks()
        {
            //addClass("myClass");
            List<string> generatedList = new List<string>();
            foreach (Category pdfClass in Const.categories)
            {
                if (!pdfClass.SiteClass.Equals("mini") && !pdfClass.SiteClass.Equals("carriers_9") && (pdfClass.PdfClass[2] == 'A'))
                    generatedList.Add(string.Concat(AddTransm("Automatic"), "&filter_carclass=" + pdfClass.SiteClass));
                else
                    generatedList.Add(string.Concat(AddTransm("Manual"), "&filter_carclass=" + pdfClass.SiteClass));
            }
            return generatedList;
        }

        public void InitDate(string day)// set pu date & do date = pu date + 1
        {
            // ---rental cars---
            //extract pu day
            string pattern = "puDay=\\d+";
            string replacement = "puDay=" + day;
            Regex rgx = new Regex(pattern);
            SetSiteName(rgx.Replace(siteName, replacement));

            //extract do day
            pattern = "doDay=\\d+";
            int doDay = Int32.Parse(day) + 1;
            replacement = "doDay=" + doDay;
            rgx = new Regex(pattern);
            SetSiteName(rgx.Replace(siteName, replacement));
        }

        public void SetTime(int puHuor, int puMinute, int doHuor, int doMinute)
        {
            //extract pu hour
            string pattern = "puHour=\\d+";
            string replacement = "puHour=" + puHuor;
            Regex rgx = new Regex(pattern);

            SetSiteName(rgx.Replace(siteName, replacement));

            //extract do hour
            pattern = "doHour=\\d+";
            replacement = "doHour=" + doHuor;
            rgx = new Regex(pattern);

            SetSiteName(rgx.Replace(siteName, replacement));

            //extract do minute
            pattern = "doMinute=\\d+";
            replacement = "doMinute=" + doMinute;
            rgx = new Regex(pattern);

            SetSiteName(rgx.Replace(siteName, replacement));

            //extract pu minute
            pattern = "puMinute=\\d+";
            replacement = "puMinute=" + puMinute;
            rgx = new Regex(pattern);

            SetSiteName(rgx.Replace(siteName, replacement));
        }

        public override void InitDate(DateTime date)// set pu date & do date = pu date + 1
        {
            if (siteName.Contains("rentalcars"))
            {
                string pattern = "puDay=\\d+";
                string replacement = "puDay=" + date.Day;
                Regex rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "doDay=\\d+";
                replacement = "doDay=" + (date.Day + 1);
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "doMonth=\\d+";
                replacement = "doMonth=" + date.Month;
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "puMonth=\\d+";
                replacement = "puMonth=" + date.Month;
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "doYear=\\d+";
                replacement = "doYear=" + date.Year;
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "puYear=\\d+";
                replacement = "puYear=" + date.Year;
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));
            }
        }

        public override void SetDoDay(DateTime date)// set do date
        {
            if (siteName.Contains("rental"))
            {
                string pattern = "doDay=\\d+";
                string replacement = "doDay=" + date.Day;
                Regex rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "doMonth=\\d+";
                replacement = "doMonth=" + date.Month;
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

                pattern = "doYear=\\d+";
                replacement = "doYear=" + date.Year;
                rgx = new Regex(pattern);
                SetSiteName(rgx.Replace(siteName, replacement));

            }
            else
            {

            }
        }

        public override string GetPuDay()
        {
            return GetDocumentDetails("puDay=[^&]*");
        }

        public override string GetPuMonth()
        {
            return GetDocumentDetails("puMonth=[^&]*");
        }

        public override string GetPuYear()
        {
            return GetDocumentDetails("puYear=[^&]*");
        }

        public override string GetCity()
        {
            return GetDocumentDetails("dropCity=[^&]*");
        }

        private string GetDocumentDetails(string pattern)
        {
            Match matchDetails = Regex.Match(GetSiteName(), pattern);

            return matchDetails.Captures[0].Value.Substring(pattern.Length - 5, matchDetails.Captures[0].Value.Length - (pattern.Length - 5));
        }
    }



}
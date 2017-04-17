using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
    
namespace IndividualLogins.Controllers.App_Code
{
    public abstract class SiteBase
    {
        protected string siteName;
        protected string Title;
        public void SetSiteName(string siteName1)
        {
            siteName = siteName1;
            SetTitle(ParseTitle(siteName1));
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public virtual string GetTitle()
        {
            return Title;
        }
        public string GetSiteName()
        {
            return siteName;
        }

        public List<string> GetGeneratedLinksByDate(DateTime sDate, DateTime eDate)
        {
            List<string> generatedList = new List<string>();
            InitDate(sDate);
            if ((eDate - sDate).Days == 0)
            {
                SetDoDay(sDate.AddDays(1));
                generatedList.Add(GetSiteName());
                return generatedList;
            }
            for (int i = 1; i <= (eDate - sDate).Days; i++)
            {
                SetDoDay(sDate.AddDays(i));
                generatedList.Add(GetSiteName());
            }
            return generatedList;
        }

        public string AddZero(int num)
        {
            return num > 9? "" + num: "0" + num;
        }

        public string GetDateString(DateTime date, string separator)
        {
            return date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day) + "T" + AddZero(date.Hour) + separator + AddZero(date.Minute);
        }
        public abstract void InitDate(DateTime date);

        public abstract void SetDoDay(DateTime day);

        public abstract string GetPuDay();

        public abstract string GetPuMonth();

        public abstract string GetPuYear();

        public virtual string ParseTitle(string siteName)
        {
            string pattern = ".*(\\.).{3}\\/";
            Match matchDetails = Regex.Match(siteName, pattern);

            string title = matchDetails.Captures[0].Value.Substring(pattern.Length - 5, matchDetails.Captures[0].Value.Length - (pattern.Length - 5));

            return title.Substring(4, title.Length - 9);
        }

        public abstract string GetCity();
    }


}
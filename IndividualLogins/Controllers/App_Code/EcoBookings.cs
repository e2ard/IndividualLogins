using IndividualLogins.Controllers.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace IndividualLogins.Controllers.App_Code
{
    public class EcoBookings : SiteBase
    {
        public EcoBookings(string site)
        {
            SetSiteName(site);
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

        public override void InitDate(DateTime d)// set pu date & do date = pu date + 1
        {
            Regex rgx = new Regex("(\\d){12}");
            SetSiteName(rgx.Replace(GetSiteName(), "" + d.Year + AddZero(d.Month) + AddZero(d.Day) + AddZero(d.Hour) + AddZero(d.Minute), 1));
        }

        public override string GetTitle()
        {
            return "EcoBookings";
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
                //generatedList.Add("https://www.economybookings.com/lt/cars/results?pco=4408&pci=110670&plc=233100&dco=4408&dci=110670&dlc=233100&py=2016&pm=12&pd=20&dy=2016&dm=12&dd=21&pt=1000&dt=1000&cr=127&age=35&crcy=EUR&lang=lt&a=1&QueryR=Rome+Airport+Fiumicino+(FCO)");
                //generatedList.Add("https://www.economybookings.com/lt/cars/results?pco=4408&pci=110670&plc=233100&dco=4408&dci=110670&dlc=233100&py=2016&pm=12&pd=20&dy=2016&dm=12&dd=22&pt=1000&dt=1000&cr=127&age=35&crcy=EUR&lang=lt&a=1&QueryR=Rome+Airport+Fiumicino+(FCO)");
                //generatedList.Add("https://www.economybookings.com/lt/cars/results?pco=4408&pci=110670&plc=233100&dco=4408&dci=110670&dlc=233100&py=2016&pm=12&pd=20&dy=2016&dm=12&dd=23&pt=1000&dt=1000&cr=127&age=35&crcy=EUR&lang=lt&a=1&QueryR=Rome+Airport+Fiumicino+(FCO)");
                //generatedList.Add("https://www.economybookings.com/lt/cars/results?pco=&pci=&plc=233100&dco=&dci=&dlc=233100&py=2016&pm=12&pd=20&dy=2016&dm=12&dd=24&pt=1000&dt=1000&age=35&crcy=EUR&lang=lt&a=1&QueryF=Rome+Airport+Fiumicino+(FCO)");
                //generatedList.Add("https://www.economybookings.com/lt/cars/results?pco=4408&pci=110670&plc=233100&dco=4408&dci=110670&dlc=233100&py=2016&pm=12&pd=20&dy=2016&dm=12&dd=25&pt=1000&dt=1000&cr=127&age=35&crcy=EUR&lang=lt&a=1&QueryR=Rome+Airport+Fiumicino+(FCO)");
            }
            return generatedList;
        }

        
        public override void SetDoDay(DateTime d)// set do date
        {
            Regex rgx = new Regex("\\d{12}/35%");
            SetSiteName(rgx.Replace(GetSiteName(), "" + d.Year + AddZero(d.Month) + AddZero(d.Day) + AddZero(d.Hour) + AddZero(d.Minute) + "/35%", 1));
        }
    }
}
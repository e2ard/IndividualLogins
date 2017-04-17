using IndividualLogins.Controllers.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndividualLogins.Controllers.App_Code
{
    public class Vehicle : SiteBase
    {
        private string ecoBoking;

        public Vehicle(string ecoBoking)
        {
            this.ecoBoking = ecoBoking;
        }

        public override string GetCity()
        {
            return "Unnamed";
        }

        public override string GetPuDay()
        {
            return "12";
        }

        public override string GetPuMonth()
        {
            return "12";
        }

        public override void InitDate(DateTime date)
        {
           
        }

        public override void SetDoDay(DateTime day)
        {
            
        }

        public override string GetTitle()
        {
            return "Vehicle";
        }

        public override string GetPuYear()
        {
            return "2016";
        }
    }
}
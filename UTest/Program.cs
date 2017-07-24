using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndividualLogins.Models;
using IndividualLogins.Controllers.App_Code;
using System.Threading;
using System.Diagnostics;
using HtmlAgilityPack;

namespace UTest
{
    class Program
    {
        
        static void Main(string[] args)
        {

            //new UTest().CalculatePricesTest();
            new UTest().CalculateTime();

        }
    }

    class UTest
    {
        public void CalculateTime()
        {
            DateTime now = DateTime.Now;
            JSourceReader reader = new JSourceReader();
            HtmlNodeCollection str = reader.GetResultGroup("http://www.rentalcars.com/SearchResults.do?enabler=&country=Latvija&doYear=2017&distance=10&ftsEntry=1472408&city=Ryga&driverage=on&doFiltering=false&dropCity=Ryga&dropFtsType=A&ftsAutocomplete=Ryga+Oro+Uostas+(RIX)%2c+Ryga%2c+Latvija&driversAge=30&dropFtsAutocomplete=Ryga+Oro+Uostas+(RIX)%2c+Ryga%2c+Latvija&dropFtsLocationName=Ryga+Oro+Uostas+(RIX)&fromLocChoose=true&filterTo=1000&dropLocationName=Ryga+Oro+Uostas+(RIX)&dropCountryCode=lv&ftsType=A&doMinute=0&countryCode=lv&puYear=2017&puSameAsDo=on&ftsLocationSearch=24281&dropFtsSearch=L&locationName=Ryga+Oro+Uostas+(RIX)&puMinute=0&ftsInput=riga&doDay=1&searchType=&filterFrom=0&coordinates=56.9236%2c23.9711&puMonth=5&dropLocation=24281&dropFtsInput=riga&doHour=10&dropCountry=Latvija&dropCoordinates=56.9236%2c23.9711&ftsLocationName=Ryga+Oro+Uostas+(RIX)&ftsSearch=L&puDay=31&dropFtsLocationSearch=24281&puHour=10&location=24281&dropFtsEntry=1472408&doMonth=6&filterName=CarCategorisationSupplierFilter");
            Console.WriteLine("duration " + (DateTime.Now - now).TotalSeconds);
            Console.ReadLine();
        }
        public void CalculatePricesTest()
        {
            string brokerName = "JIG";
            //  for (int i = 0; i < Classes.Count(); i++)
            {
                List<JOffer> categoryOffers = GetTestOffers();
                List<JOffer> higherOffers = GeHigherOffers();
                List<float> fuseRates = GetFuseRatesTest();
                PricingClass[] classes = new PricingClass[5];
                PricingHelper hp = new PricingHelper(new PricingModel(), classes);
                List<float> priceList = hp.CalculatePrices(categoryOffers, higherOffers, fuseRates, brokerName);

                Console.WriteLine("-----------------");

                for (int i = 0; i < priceList.Count; i++)
                {
                    JOffer tempOffer = categoryOffers.Count > i ? categoryOffers.ElementAt(i): null;
                    float price = tempOffer != null ? tempOffer.price : 0;
                    float gmPrice = tempOffer != null ? tempOffer.gmPrice : 0;

                    Console.WriteLine(i + " price " + price + " gmprice " + gmPrice + " fuse: " + fuseRates.ElementAt(i) + " new price: " + priceList.ElementAt(i));
                }
                Console.WriteLine("-----------------");
                Console.ReadLine();
            }
        }

        private List<JOffer> GeHigherOffers()
        {
            List<JOffer> offers = new List<JOffer>();

            JOffer o1 = new JOffer("Budget", 35.2f);
            o1.SetGM("Green");
            o1.SetGMPrice(23f);
            offers.Add(o1);

            o1 = new JOffer("Avis", 46.2f);
            o1.SetGM("Green");
            o1.SetGMPrice(30f);
            offers.Add(o1);

            o1 = new JOffer("SIxt", 58.2f);
            o1.SetGM("Green");
            o1.SetGMPrice(55f);
            offers.Add(o1);

            return offers;
        }

        private List<float> GetFuseRatesTest()
        {
            List<float> flist = new List<float>();

            flist.Add(19.01f);
            flist.Add(29.01f);
            flist.Add(39.01f);
            flist.Add(49.01f);
            flist.Add(59.01f);

            for(int i = 5; i < 30; i++)
            {
                flist.Add(i * 10.06f);
            }

            return flist;
        }

        private List<JOffer> GetTestOffers()
        {
            List<JOffer> offers = new List<JOffer>();

            JOffer o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(23f);
            offers.Add(o1);

            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(30f);
            offers.Add(o1);

            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(23f);
            offers.Add(o1);

            o1 = new JOffer("Europcar", 61.2f);
            o1.SetGM("Green");
            o1.SetGMPrice(88f);
            offers.Add(o1);

            o1 = new JOffer("Europcar", 61.2f);
            offers.Add(o1);

            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(88f);
            offers.Add(o1);

            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(88f);
            offers.Add(o1);

            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(88f);
            offers.Add(o1);


            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(108f);

            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(118f);
            offers.Add(o1);


            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(130f);


            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(154f);
            offers.Add(o1);


            o1 = new JOffer();
            o1.SetGM("Green");
            o1.SetGMPrice(180f);

            offers.Add(o1);
            offers.Add(o1);
            offers.Add(o1);
            return offers;
        }

    }
}

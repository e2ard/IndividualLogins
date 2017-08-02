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
            new UTest().CalculatePricesTest();
            new UTest().CalculateTime();

        }
    }

    class UTest
    {
        static string siteString = "https://otageo.cartrawler.com/cartrawlerota/json?callback=angular.callbacks._3&msg={%22@Target%22:%22Production%22,%22@PrimaryLangID%22:%22en%22,%22POS%22:{%22Source%22:[{%22@ERSP_UserID%22:%22MP%22,%22@ISOCurrency%22:%22EUR%22,%22RequestorID%22:{%22@Type%22:%2216%22,%22@ID%22:%22643826%22,%22@ID_Context%22:%22CARTRAWLER%22}},{%22RequestorID%22:{%22@Type%22:%2216%22,%22@ID%22:%220410201033167920303%22,%22@ID_Context%22:%22CUSTOMERID%22}},{%22RequestorID%22:{%22@Type%22:%2216%22,%22@ID%22:%220410201033167920303%22,%22@ID_Context%22:%22ENGINELOADID%22}},{%22RequestorID%22:{%22@Type%22:%2216%22,%22@ID%22:%22643826%22,%22@ID_Context%22:%22PRIORID%22}},{%22RequestorID%22:{%22@Type%22:%2216%22,%22@ID%22:%223%22,%22@ID_Context%22:%22BROWSERTYPE%22}}]},%22@xmlns%22:%22http:%2F%2Fwww.opentravel.org%2FOTA%2F2003%2F05%22,%22@xmlns:xsi%22:%22http:%2F%2Fwww.w3.org%2F2001%2FXMLSchema-instance%22,%22@Version%22:%221.005%22,%22VehAvailRQCore%22:{%22@Status%22:%22Available%22,%22VehRentalCore%22:{%22@PickUpDateTime%22:%222017-12-23T20:00:00%22,%22@ReturnDateTime%22:%222017-12-29T20:00:00%22,%22PickUpLocation%22:{%22@CodeContext%22:%22CARTRAWLER%22,%22@LocationCode%22:%223224%22},%22ReturnLocation%22:{%22@CodeContext%22:%22CARTRAWLER%22,%22@LocationCode%22:%223224%22}},%22VehPrefs%22:{%22VehPref%22:{%22VehClass%22:{%22@Size%22:%220%22}}},%22DriverType%22:{%22@Age%22:30}},%22VehAvailRQInfo%22:{%22Customer%22:{%22Primary%22:{%22CitizenCountryName%22:{%22@Code%22:%22LT%22}}},%22TPA_Extensions%22:{%22showBaseCost%22:true,%22ConsumerIP%22:%2278.56.135.139%22,%22GeoRadius%22:5,%22Window%22:{%22@name%22:%22Ryanair%2520-%2520Car%2520Hire%22,%22@engine%22:%22CTABE-V5.1%22,%22@svn%22:%225.1.11-3%22,%22@region%22:%22en%22,%22@patVal%22:0,%22UserAgent%22:%22Mozilla%2F5.0+%28Windows+NT+6.3;+WOW64;+rv:45.0%29+Gecko%2F20100101+Firefox%2F45.0%22,%22BrowserName%22:%22firefox%22,%22BrowserVersion%22:%2245%22,%22URL%22:%22https:%2F%2Fcar-hire.ryanair.com%2Fen-gb%2Fbook%3FclientId%3D643826%26currency%3DGBP%26age%3D30%26pickupName%3DVilnius%2520Airport%26returnName%3DVilnius%2520Airport%26countryID%3DLT%26residencyId%3DLT%26pickupDate%3D10%26pickupMonth%3D03%26pickupYear%3D2016%26pickupHour%3D10%26pickupMinute%3D0%26returnDate%3D10%26returnMonth%3D04%26returnYear%3D2016%26returnHour%3D10%26returnMinute%3D0%26pickupID%3D3224%26returnID%3D3224%22},%22RefID%22:{}}}}&type=OTA_VehAvailRateRQ";
        SiteBase site = new Trawler(siteString);
        SearchFilters sf = new SearchFilters { PuDate = new DateTime(2017, 11, 01), DoDate = new DateTime(2017, 11, 13), Location = 1, Source = 2};
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
            Dictionary<string, Dictionary<string, JOffer>> pdfRates = new Rates().GetRates(sf, out site);
            string brokerName = "JIG";
            //  for (int i = 0; i < Classes.Count(); i++)
            {
                PricingHelper ph = new PricingHelper();

                List<JOffer> categoryOffers = new PricingHelper().GetMiniRatesList(pdfRates, "EconomyA");
                List<JOffer> higherOffers = new PricingHelper().GetMiniRatesList(pdfRates, "CompactM");
                List<float> fuseRates = new CsvHelper("314141").GetFuseRates(brokerName);

                PricingClass[] classes = new PricingClass[5];
                PricingHelper hp = new PricingHelper(new PricingModel(), classes);
                List<float> priceList = hp.CalculatePrices(categoryOffers, higherOffers, fuseRates, brokerName);

                Console.WriteLine("-----------------");

                for (int i = 0; i < priceList.Count; i++)
                {
                    JOffer tempOffer = categoryOffers.Count > i ? categoryOffers.ElementAt(i): null;
                    float price = tempOffer != null ? tempOffer.GetMinPrice() : 0;
                    float gmPrice = tempOffer != null ? tempOffer.GetMinGmPrice() : 0;

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

            o1 = new JOffer();
            o1.SetGM("");
            o1.SetGMPrice(0);

            o1 = new JOffer();
            o1.SetGM("");
            o1.SetGMPrice(0);

            offers.Add(o1);
            offers.Add(o1);
            offers.Add(o1);
            return offers;
        }

    }
}

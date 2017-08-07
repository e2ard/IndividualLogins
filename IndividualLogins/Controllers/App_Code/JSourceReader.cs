using HtmlAgilityPack;
using IndividualLogins.Models;
using IndividualLogins.Models.NlogTest.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static JOffer;

namespace IndividualLogins.Controllers.App_Code
{

    public class JSourceReader
    {
        public static string ipStr = "lv2.nordvpn.com";//"193.105.240.1";
        public static int port = 80;//8080
        public static bool addProxy = true;//if true then add
        public static string user = "edvard.naus@gmail.com";
        public static string pass = "421c3421c3";
        private List<Cars> cars = new List<Cars>();
        public JSourceReader() { }
        public string GetSource(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
                    client.Headers.Add("Accept", "*/*");
                    client.Headers.Add("Accept-Language", "en-gb,en;q=0.5");
                    client.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                    if (addProxy)
                    {
                        WebProxy proxy = new WebProxy(ipStr, port);
                        proxy.Credentials = new NetworkCredential(user, pass);
                        client.Proxy = proxy;
                    }
                    return client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error("-JSR.GetSource-" + ex.Message + "\n " + ex.InnerException + "\n" + ex.StackTrace);
                return null;
            }
        }
        public HtmlNodeCollection GetResultGroup(string site)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(GetSource(site));

            string[] carOfferStrgs = { "car-result group ", "search-result txt-grey-7 ", "carResultDiv " };
            HtmlNodeCollection offersFound = null;
            for (int i = 0; i < carOfferStrgs.Count() && offersFound == null; i++)
            {
                offersFound = doc.DocumentNode.SelectNodes(".//div[contains(@class,'" + carOfferStrgs[i] + "')]");
            }
            return offersFound;
        }
        public List<SupplierNew> ExtractOffers(HtmlNodeCollection resultGroups)
        {
            List<SupplierNew> offers = new List<SupplierNew>();
            if (resultGroups != null)
            {
                foreach (HtmlNode mainNode in resultGroups)
                {
                    string price = ParsePrice(mainNode);
                    string supplier = ParseSupplier(mainNode);
                    string category = ParseCategory(mainNode);
                    string transm = ParseTransm(mainNode);
                    string seats = ParseSeats(mainNode);
                    string carName = ParseCarname(mainNode);

                    SupplierNew o = new SupplierNew(supplier, price, category, transm, seats, carName);
                    if (carName.ToLower().Contains("renault clio estate") && o.Transmission.Equals("A"))
                        o.Category = "Economy";
                    offers.Add(o);
                }
            }
            else
                Debug.WriteLine("JSOURCEREDAER 0 offer number ");

            return offers;
        }
        public string ParseCarname(HtmlNode mainNode)
        {
            string carName = string.Empty;
            string[] carNameStrgs = { ".//td[contains(@class,'carResultRow_CarSpec')]/h2" };
            HtmlNode carNameNode = null;
            for (int i = 0; i < carNameStrgs.Count() && carNameNode == null; i++)
            {
                carNameNode = mainNode.SelectSingleNode(carNameStrgs[i]);
                if (carNameNode != null)
                    return carNameNode.InnerText.Split('&')[0];
            }
            if (carNameNode == null)
                Debug.WriteLine("-------------> carName not pursed");
            return null;
        }
        public string ParseSeats(HtmlNode mainNode)
        {
            string seats = string.Empty;
            string[] seatsStrgs = { ".//li[contains(@class,'result_seats')]", ".//span[contains(@class,'class mini')]", ".//li[contains(@class,'carResultRow_CarSpec_Seats')]" };
            HtmlNode seatsNode = null;
            for (int i = 0; i < seatsStrgs.Count() && seatsNode == null; i++)
            {
                seatsNode = mainNode.SelectSingleNode(seatsStrgs[i]);
                if (seatsNode != null)
                    return seatsNode.InnerText;
            }
            if (seatsNode == null)
                Debug.WriteLine("-------------> seats not pursed");
            return null;
        }
        public string ParseTransm(HtmlNode mainNode)
        {
            //transmission ------------------------------------------------
            string transm = string.Empty;
            string[] transmStrgs = { ".//li[contains(@class,'result_trans')]", ".//span[contains(@class,'class mini')]", ".//ul[contains(@class, 'carResultRow_CarSpec-tick')]/li[2]" };
            HtmlNode transmNode = null;
            for (int i = 0; i < transmStrgs.Count() && transmNode == null; i++)
            {
                transmNode = mainNode.SelectSingleNode(transmStrgs[i]);
                if (transmNode != null)
                    return transmNode.InnerText;
            }
            if (transmNode == null)
                Debug.WriteLine("-------------> transm not pursed");
            return null;
        }
        public string ParseSupplier(HtmlNode mainNode)
        {
            string supplier = string.Empty;
            string[] supplierStrgs = { ".//div[@class='supplier_id']/img", ".//div[@class='col dbl info-box supplier']/img", ".//div[@class='carResultRow_OfferInfo_Supplier-wrap']/img" };
            HtmlNode supplierNode = null;
            for (int i = 0; i < supplierStrgs.Count() && supplierNode == null; i++)
            {
                supplierNode = mainNode.SelectSingleNode(supplierStrgs[i]);
                if (supplierNode != null)
                    return supplierNode.Attributes["title"].Value;
            }
            if (supplierNode == null)
                Debug.WriteLine("Suppliernot pursed -------------------------------");
            return null;
        }
        public string ParseCategory(HtmlNode mainNode)
        {
            //category ------------------------------------------------
            string category = string.Empty;
            string[] categoryStrs = { ".//p[contains(@class,'bg-yellow-5')]", ".//span[contains(@class,'class mini')]", ".//span[contains(@class,'carResultRow_CarSpec_CarCategory')]" };
            HtmlNode categoryNode = null;
            for (int i = 0; i < categoryStrs.Count() && categoryNode == null; i++)
            {
                categoryNode = mainNode.SelectSingleNode(categoryStrs[i]);
                if (categoryNode != null)
                    return categoryNode.InnerText;
            }
            if (categoryNode == null)
                Debug.WriteLine("-------------> category not pursed");
            return null;
        }
        public string ParsePrice(HtmlNode mainNode)
        {
            string[] priceStrgs = { ".//p[@class='now ']", ".//p[@class='now']", ".//span[@class='carResultRow_Price-now']" };
            HtmlNode priceNode = null;
            string price = string.Empty;
            for (int i = 0; i < priceStrgs.Count() && priceNode == null; i++)
            {
                priceNode = mainNode.SelectSingleNode(priceStrgs[i]);
                if (priceNode != null)
                    return priceNode.InnerText;
            }

            if (priceNode == null)
                Debug.WriteLine("--------------->Price not pursed----------------");

            return null;
        }
        public Dictionary<string, JOffer> GetMap(List<SupplierNew> offers)
        {
            Dictionary<string, JOffer> dayOffers = new Dictionary<string, JOffer>();
            foreach (SupplierNew o in offers)
            {
                string offerKey = o.Category + o.Transmission;
                if (offerKey.Equals("People CarrierM") && !o.Seats.Equals("9") || o.Category.Contains("skip"))
                {
                    AddCar(o);
                    continue;
                }

                if (dayOffers.ContainsKey(offerKey))
                {
                    dayOffers[offerKey].AddSupplier(o);
                }
                else
                {
                    dayOffers[offerKey] = new JOffer();
                    dayOffers[offerKey].AddSupplier(o);
                }
            }
            return dayOffers;
        }
        public void AddCar(SupplierNew s)
        {
            using(RatesDBContext ctx = new RatesDBContext())
            {
                Cars c = ctx.Cars.FirstOrDefault(o => o.CarName.Contains(s.CarName));
                if (c == null)
                {
                    ctx.Cars.Add(new Cars
                    {
                        CarName = s.CarName,
                        Category = s.Category.Contains("skip") ? s.Category.Substring(4) : s.Category,
                        Seats = s.Seats,
                        Transmission = s.Transmission,
                        SupplierName = s.SupplierName,
                        SupplierType = s.SupplierType,
                        IsAssigned = true
                    });
                    ctx.SaveChanges();
                }
            }
        }
        public List<SupplierNew> GetNorwRates(string url)
        {
            string responseFromServer = GetSource(url);

            int remove = Regex.Match(responseFromServer, "(\\w)*\\[\\],").Captures[0].Index + 3;

            string subJson = "{" + responseFromServer.Substring(remove, responseFromServer.Length - 2 - remove);

            JToken entireJson = JToken.Parse(subJson);

            JArray vehVendorAvails = entireJson["VehAvailRSCore"]["VehVendorAvails"].Value<JArray>();// Get suppliers

            List<SupplierNew> offers = new List<SupplierNew>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["Vendor"]["@CompanyShortName"].ToString();
                foreach (var vehicle in item["VehAvails"])
                {
                    string category = vehicle["VehAvailCore"]["Vehicle"]["@Code"].ToString();// category
                    string price = vehicle["VehAvailCore"]["TotalCharge"]["@RateTotalAmount"].ToString();//price
                    string transm = vehicle["VehAvailCore"]["Vehicle"]["@TransmissionType"].ToString().Substring(0,1);//transm
                    string carName = vehicle["VehAvailCore"]["Vehicle"]["VehMakeModel"]["@Name"].ToString();// carname
                    float seats = int.Parse(vehicle["VehAvailCore"]["Vehicle"]["@PassengerQuantity"].ToString());//seats
                    SupplierNew offer = new SupplierNew(supplier, price);
                    offer.CarName = carName;
                    offer.SetCategory(category);
                    //offer.Transmission = transm;
                    offer.Seats = seats;
                    offers.Add(offer);
                }
            }
            return offers;
        }

        public List<SupplierNew> GetScannerRates(string url)
        {
            string responseFromServer = GetSource(url);

            int remove = Regex.Match(responseFromServer, "(\\w)*\\[\\],").Captures[0].Index + 3; ;

            string subJson = "{" + responseFromServer.Substring(remove, responseFromServer.Length - 2 - remove);

            JToken entireJson = JToken.Parse(subJson);

            JArray vehVendorAvails = entireJson["VehAvailRSCore"]["VehVendorAvails"].Value<JArray>();// Get suppliers

            List<SupplierNew> offers = new List<SupplierNew>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["Vendor"]["@CompanyShortName"].ToString();
                foreach (var vehicle in item["VehAvails"])
                {
                    string category = vehicle["VehAvailCore"]["Vehicle"]["@Code"].ToString();// category
                    string price = vehicle["VehAvailCore"]["TotalCharge"]["@RateTotalAmount"].ToString();//price
                    SupplierNew offer = new SupplierNew(supplier, price);
                    offer.SetCategory(category);
                    offers.Add(offer);
                }
            }
            return offers;
        }

    }
}
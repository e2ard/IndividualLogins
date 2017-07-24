using HtmlAgilityPack;
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

namespace IndividualLogins.Controllers.App_Code
{

    public class JSourceReader
    {
        public static string ipStr = "lv2.nordvpn.com";//"193.105.240.1";
        public static int port = 80;//8080
        public static bool addProxy = true;//if true then add
        public static string user = "edvard.naus@gmail.com";
        public static string pass = "421c3421c3";

        public JSourceReader() { }

        public JOffer GetMinOffer(List<JOffer> offers)
        {
            float minPrice = 999999;
            String minSupplier = null;
            float minGMPrice = 9999999;
            String minGMSupplier = null;
            foreach (JOffer of in offers)
            {
                String supplier = of.GetSupplier().ToLower();
                if (!(supplier.Equals("green motion"))
                     && (of.GetPrice() < minPrice))
                {

                    minPrice = of.GetPrice();
                    minSupplier = of.GetSupplier();
                }
                else if (supplier.Equals("green motion")
                    && (of.GetPrice() < minGMPrice))
                {
                    minGMPrice = of.GetPrice();
                    minGMSupplier = "GM";
                }

            }
            JOffer offer = new JOffer();
            offer.SetPrice(minPrice);
            offer.SetSupplier(minSupplier);
            offer.SetGMPrice(minGMPrice);
            offer.SetGM(minGMSupplier);
            offer.SetSiteName(offer.GetSiteName());
            return offer;
        }

        public HtmlNodeCollection GetResultGroup(string site)
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
                    client.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                    var responseStream = new GZipStream(client.OpenRead(site), CompressionMode.Decompress);
                    var reader = new StreamReader(responseStream);
                    string source = reader.ReadToEnd();

                    //string source = client.DownloadString(site);
                    HtmlDocument doc = new HtmlDocument();//retrieve page source tags
                    doc.LoadHtml(source);

                    string[] carOfferStrgs = { "car-result group ", "search-result txt-grey-7 ", "carResultDiv " };
                    HtmlNodeCollection offersFound = null;
                    for (int i = 0; i < carOfferStrgs.Count() && offersFound == null; i++)
                    {
                        offersFound = doc.DocumentNode.SelectNodes(".//div[contains(@class,'" + carOfferStrgs[i] + "')]");
                    }
                    return offersFound;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Offers not found");
                Log.Instance.Error("--- " + ex.Message + "\n " + ex.InnerException + "\n" + ex.StackTrace);
                return null;
            }
        }

        public List<JOffer> ExtractOffers(HtmlNodeCollection resultGroups)
        {
            List<JOffer> offers = new List<JOffer>();
            if (resultGroups != null)
            {
                foreach (HtmlNode mainNode in resultGroups)
                {
                    //price ------------------------------------------------
                    string[] priceStrgs = { ".//p[@class='now ']", ".//p[@class='now']", ".//span[@class='carResultRow_Price-now']" };
                    HtmlNode priceNode = null;
                    string price = string.Empty;
                    for (int i = 0; i < priceStrgs.Count() && priceNode == null; i++)
                    {
                        priceNode = mainNode.SelectSingleNode(priceStrgs[i]);
                        if (priceNode != null)
                            price = priceNode.InnerText;
                    }
                    if (priceNode == null)
                        Debug.WriteLine("--------------->Price not pursed----------------");
                    //supplier ------------------------------------------------
                    string supplier = string.Empty;
                    string[] supplierStrgs = { ".//div[@class='supplier_id']/img", ".//div[@class='col dbl info-box supplier']/img", ".//div[@class='carResultRow_OfferInfo_Supplier-wrap']/img" };
                    HtmlNode supplierNode = null;
                    for (int i = 0; i < supplierStrgs.Count() && supplierNode == null; i++)
                    {
                        supplierNode = mainNode.SelectSingleNode(supplierStrgs[i]);
                        if (supplierNode != null)
                            supplier = supplierNode.Attributes["title"].Value;
                    }
                    if (supplierNode == null)
                        Debug.WriteLine("Suppliernot pursed -------------------------------");
                    //category ------------------------------------------------
                    string category = string.Empty;
                    string[] categoryStrs = { ".//p[contains(@class,'bg-yellow-5')]", ".//span[contains(@class,'class mini')]", ".//span[contains(@class,'carResultRow_CarSpec_CarCategory')]" };
                    HtmlNode categoryNode = null;
                    for (int i = 0; i < categoryStrs.Count() && categoryNode == null; i++)
                    {
                        categoryNode = mainNode.SelectSingleNode(categoryStrs[i]);
                        if (categoryNode != null)
                            category = categoryNode.InnerText;
                    }
                    if (categoryNode == null)
                        Debug.WriteLine("-------------> category not pursed");
                    //transmission ------------------------------------------------
                    string transm = string.Empty;
                    string[] transmStrgs = { ".//li[contains(@class,'result_trans')]", ".//span[contains(@class,'class mini')]", ".//ul[contains(@class, 'carResultRow_CarSpec-tick')]/li[2]" };
                    HtmlNode transmNode = null;
                    for (int i = 0; i < transmStrgs.Count() && transmNode == null; i++)
                    {
                        transmNode = mainNode.SelectSingleNode(transmStrgs[i]);
                        if (transmNode != null)
                            transm = transmNode.InnerText;
                    }
                    if (transmNode == null)
                        Debug.WriteLine("-------------> transm not pursed");
                    //seats
                    string seats = string.Empty;
                    string[] seatsStrgs = { ".//li[contains(@class,'result_seats')]", ".//span[contains(@class,'class mini')]", ".//li[contains(@class,'carResultRow_CarSpec_Seats')]" };
                    HtmlNode seatsNode = null;
                    for (int i = 0; i < seatsStrgs.Count() && seatsNode == null; i++)
                    {
                        seatsNode = mainNode.SelectSingleNode(seatsStrgs[i]);
                        if (seatsNode != null)
                            seats = seatsNode.InnerText;
                    }
                    if (seatsNode == null)
                        Debug.WriteLine("-------------> seats not pursed");

                    //car name ------------------------------------------------
                    string carName = string.Empty;
                    string[] carNameStrgs = { ".//td[contains(@class,'carResultRow_CarSpec')]" };
                    HtmlNode carNameNode = null;
                    for (int i = 0; i < carNameStrgs.Count() && carNameNode == null; i++)
                    {
                        carNameNode = mainNode.SelectSingleNode(carNameStrgs[i]);
                        if (carNameNode != null)
                            carName = carNameNode.InnerText.Split('&')[0];
                    }
                    if (carNameNode == null)
                        Debug.WriteLine("-------------> carName not pursed");

                    JOffer o = new JOffer(supplier, price, category, transm, seats);
                    if (carName.ToLower().Contains("renault clio estate") && o.transmission.Equals("A"))
                        o.category = "Economy";
                    offers.Add(o);
                }
            }
            else
                Debug.WriteLine("JSOURCEREDAER 0 offer number ");

            return offers;
        }

        public Dictionary<string, JOffer> GetMap(List<JOffer> offers)
        {
            Dictionary<string, JOffer> dayOffers = new Dictionary<string, JOffer>();
            foreach (JOffer o in offers)
            {
                string offerKey = o.category + o.transmission;
                if (offerKey.Equals("People CarrierM") && !o.seats.Equals("9"))
                   continue;
               
                if (dayOffers.ContainsKey(offerKey))
                {
                    if (o.GetPrice() < dayOffers[offerKey].GetPrice() && o.GetPrice() != 0 || dayOffers[offerKey].GetPrice() == 0)
                    {
                        dayOffers[offerKey].SetPrice(o.GetPrice());
                        dayOffers[offerKey].SetSupplier(o.GetSupplier());
                    }
                    if (o.GetGMPrice() < dayOffers[offerKey].GetGMPrice() && o.GetGMPrice() != 0 || dayOffers[offerKey].GetGMPrice() == 0)
                    {
                        dayOffers[offerKey].SetGMPrice(o.GetGMPrice());
                        dayOffers[offerKey].SetGM(o.GetGM());
                    }
                    if (o.GetCRPrice() < dayOffers[offerKey].GetCRPrice() && o.GetCRPrice() != 0 || dayOffers[offerKey].GetCRPrice() == 0)
                    {
                        dayOffers[offerKey].SetCRPrice(o.GetCRPrice());
                        dayOffers[offerKey].SetCR(o.GetCR());
                    }
                }
                else
                    dayOffers[offerKey] = o; // if not initialized add new offer
            }
            return dayOffers;
        }

        public Dictionary<string, JOffer> GetMapNorwegian(List<JOffer> offers)
        {
            Dictionary<string, JOffer> dayOffers = new Dictionary<string, JOffer>();
            foreach (JOffer o in offers)
            {
                string offerKey = o.category;
                if (o.category.Equals("skip"))
                    continue;
                
                if (dayOffers.ContainsKey(o.category))
                {
                    if (o.GetPrice() < dayOffers[offerKey].GetPrice() && o.GetPrice() != 0 || dayOffers[offerKey].GetPrice() == 0)
                    {
                        dayOffers[offerKey].SetPrice(o.GetPrice());
                        dayOffers[offerKey].SetSupplier(o.GetSupplier());
                    }
                    if (o.GetGMPrice() < dayOffers[offerKey].GetGMPrice() && o.GetGMPrice() != 0 || dayOffers[offerKey].GetGMPrice() == 0)
                    {
                        dayOffers[offerKey].SetGMPrice(o.GetGMPrice());
                        dayOffers[offerKey].SetGM(o.GetGM());
                    }
                    if (o.GetCRPrice() < dayOffers[offerKey].GetCRPrice() && o.GetCRPrice() != 0 || dayOffers[offerKey].GetCRPrice() == 0)
                    {
                        dayOffers[offerKey].SetCRPrice(o.GetCRPrice());
                        dayOffers[offerKey].SetCR(o.GetCR());
                    }
                }
                else
                    dayOffers[offerKey] = o; // if not initialized add new offer
            }
            return dayOffers;
        }

        public List<JOffer> GetNorwRates(string url)
        {
            WebRequest request = WebRequest.Create(url);// Create a request for the URL. 
            if (addProxy)
            {
                WebProxy proxy = new WebProxy(ipStr, port);
                proxy.Credentials = new NetworkCredential(user, pass);
                request.Proxy = proxy;
            }
            using (WebResponse response = request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())// Get the stream containing content returned by the server.
                {
                    StreamReader reader = new StreamReader(dataStream);// Open the stream using a StreamReader for easy access.

                    string responseFromServer = reader.ReadToEnd(); // Read the content.

                    int remove = Regex.Match(responseFromServer, "(\\w)*\\[\\],").Captures[0].Index + 3;

                    string subJson = "{" + responseFromServer.Substring(remove, responseFromServer.Length - 2 - remove);

                    JToken entireJson = JToken.Parse(subJson);

                    JArray vehVendorAvails = entireJson["VehAvailRSCore"]["VehVendorAvails"].Value<JArray>();// Get suppliers
                    Console.WriteLine(vehVendorAvails.Count);

                    List<JOffer> offers = new List<JOffer>();
                    foreach (var item in vehVendorAvails)
                    {
                        string supplier = item["Vendor"]["@CompanyShortName"].ToString();
                        foreach (var vehicle in item["VehAvails"])
                        {
                            string category = vehicle["VehAvailCore"]["Vehicle"]["@Code"].ToString();// category
                            string price = vehicle["VehAvailCore"]["TotalCharge"]["@RateTotalAmount"].ToString();//price
                            JOffer offer = new JOffer(supplier, price);
                            offer.SetCategory(category);
                            offers.Add(offer);
                        }
                    }

                    reader.Close();// Clean up the streams and the response.
                    return offers;
                }
            }
        }

        public List<JOffer> GetBookingOffers(string jsonStr)
        {
            JToken entireJson = JToken.Parse(jsonStr);

            JArray vehVendorAvails = entireJson["cars"].Value<JArray>();// Get suppliers
            Console.WriteLine(vehVendorAvails.Count);

            List<JOffer> offers = new List<JOffer>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["supplier"]["name"].ToString();
                string category = item["car"]["classCode"].ToString();
                string price = item["price"]["total"].ToString();
                JOffer offer = new JOffer(supplier, price);
                offer.SetCategory(category);
                offers.Add(offer);
            }

            return offers;

        }

        public List<JOffer> GetExpediaOffers(string jsonStr)
        {
            JToken entireJson = JToken.Parse(jsonStr);

            JArray vehVendorAvails = entireJson["offers"].Value<JArray>();// Get suppliers
            Console.WriteLine(vehVendorAvails.Count);

            List<JOffer> offers = new List<JOffer>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["vendor"]["name"].ToString();
                string category = item["vehicle"]["classification"]["code"].ToString() + item["vehicle"]["transmission"].ToString()[0] + "R";
                string price = item["fare"]["total"]["value"].ToString();
                JOffer offer = new JOffer(supplier, price);
                offer.SetCategory(category);
                offers.Add(offer);
            }

            return offers;

        }

        public List<JOffer> GetVehicleOffers(string jsonStr)
        {
            JToken entireJson = JToken.Parse(jsonStr);
            JArray vehVendorAvails = entireJson["cars"].Value<JArray>();// Get suppliers
            
            List<JOffer> offers = new List<JOffer>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["Supplier"]["name"].ToString();
                string category = item["Car"]["internal_class"].ToString();
                string price = item["Price"]["original_price"].ToString();
                JOffer offer = new JOffer(supplier, price);
                offer.SetCategory(category);
                offers.Add(offer);
            }

            return offers;

        }

        public string GetVehicleSource(string url)
        {
            WebRequest request = WebRequest.Create(url);

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            if (addProxy)
            {
                WebProxy proxy = new WebProxy(ipStr, port);
                proxy.Credentials = new NetworkCredential(user, pass);
                request.Proxy = proxy;
            }
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();// Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);// Read the content.

            return reader.ReadToEnd();
        }

        public List<JOffer> GetScannerRates(string url)
        {
            WebRequest request = WebRequest.Create(url);

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            if (addProxy)
            {
                WebProxy proxy = new WebProxy(ipStr, port);
                proxy.Credentials = new NetworkCredential(user, pass);
                request.Proxy = proxy;
            }
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.

            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            int remove = Regex.Match(responseFromServer, "(\\w)*\\[\\],").Captures[0].Index + 3; ;

            string subJson = "{" + responseFromServer.Substring(remove, responseFromServer.Length - 2 - remove);

            JToken entireJson = JToken.Parse(subJson);

            JArray vehVendorAvails = entireJson["VehAvailRSCore"]["VehVendorAvails"].Value<JArray>();// Get suppliers
            Console.WriteLine(vehVendorAvails.Count);

            List<JOffer> offers = new List<JOffer>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["Vendor"]["@CompanyShortName"].ToString();
                foreach (var vehicle in item["VehAvails"])
                {
                    string category = vehicle["VehAvailCore"]["Vehicle"]["@Code"].ToString();// category
                    string price = vehicle["VehAvailCore"]["TotalCharge"]["@RateTotalAmount"].ToString();//price
                    JOffer offer = new JOffer(supplier, price);
                    offer.SetCategory(category);
                    offers.Add(offer);
                    //if (offer.category.Equals("skip"))
                    //    Debug.WriteLine("Model" + vehicle["VehAvailCore"]["Vehicle"]["VehMakeModel"]["@Name"] + "Cateory " + category);//Model


                }
            }

            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return offers;
        }
    }
}
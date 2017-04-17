using HtmlAgilityPack;
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

    /// <summary>
    /// Summary description for SurceReader
    /// </summary>
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
                    string source = client.DownloadString(site);
                    //retrieve page source tags
                    HtmlDocument doc = new HtmlDocument();
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
            catch (Exception e)
            {
                Debug.WriteLine("Offers not found");
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

        public string GetBookingsSource(string site, string site2)
        {
            Console.WriteLine("-->RequestStarted");
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(site2);
                if (addProxy)
                {
                    WebProxy proxy = new WebProxy(ipStr, port);
                    proxy.Credentials = new NetworkCredential(user, pass);
                    request.Proxy = proxy;
                }
                request.KeepAlive = true;
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                request.Referer = site;
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");
                request.Headers.Set(HttpRequestHeader.Cookie, @"sLang=lt; _gat_UA-4564791-17=1; referrers=www.bookinggroup.com; _dc_gtm_UA-4564791-2=1; _dc_gtm_UA-4564791-17=1; _dc_gtm_UA-4564791-20=1; _gat_UA-4564791-2=1; _gat_UA-4564791-20=1; has_js=1; _ga=GA1.2.871746488.1481921759; uparams=QueryF");

                response = (HttpWebResponse)request.GetResponse();
                return ReadResponse(response);
            }

            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)we.Response;
                //else return false;
            }
            catch (Exception e)
            {
                if (response != null) response.Close();
                //return false;
            }
            Console.WriteLine("--<RequestFinished");
            return null;
        }

        public string GetExpediaSource(string body)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.expedia.it/carsearch/pickup/list/results");
                if (addProxy)
                {
                    WebProxy proxy = new WebProxy(ipStr, port);
                    proxy.Credentials = new NetworkCredential(user, pass);
                    request.Proxy = proxy;
                }
                request.KeepAlive = true;
                request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
                request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
                request.Accept = "*/*";
                request.Headers.Add("Origin", @"https://www.expedia.it");
                request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.Referer = "https://www.expedia.it/carsearch?dagv=1&subm=1&fdrp=&styp=2&locn=Fiumicino,%20Italia&loc2=Fiumicino,%20Italia&date1=05/01/2017&date2=08/01/2017&vend=&kind=1&time1=1200PM&time2=1200PM&ttyp=2&acop=2&rdus=10&rdct=1&drid1=179111&dlat=41.764942&dlon=12.228037";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");
                request.Headers.Set(HttpRequestHeader.Cookie, @"MC1=GUID=15f0bb182a1d4d4fae2dad2ffd44bdf0; abucket=CgUB41fucrIEy0HIF7DnAg==; MediaCookie=0%7C2859%2C2829%2CBKC%2C27485%7C2859%2C2829%2CBKC%2C27482; tpid=v.1,8; iEAPID=0; JSESSION=80fb2de0-99de-4fe4-8b84-98ab333b8142; AMCVS_C00802BE5330A8350A490D4C%40AdobeOrg=1; qualtrics_sample=false; aspp=v.1,0|||||||||||||; _cc=AbdtcyELE5sy1OuU1%2FQALcqv; IAID=70430741-4257-4054-9f67-1b61a99215a9; AMCV_C00802BE5330A8350A490D4C%40AdobeOrg=-1248264605%7CMCIDTS%7C17152%7CMCMID%7C02813494840441441920212387430884768964%7CMCAAMLH-1482519671%7C6%7CMCAAMB-1482519671%7CNRX38WO0n5BH8Th-nqAG_A%7CMCOPTOUT-1481922071s%7CNONE%7CMCAID%7CNONE; s_sq=%5B%5BB%5D%5D; cesc=%7B%7D; s_ppn=page.Car-Search; s_ppvl=page.Car-Search%2C10%2C8%2C702%2C1304%2C702%2C1366%2C768%2C1%2CP; s_cc=true; utagdb=true; utag_main=v_id:01590901eaa9001a9b3b535ddd070506d00240650086e$_sn:1$_ss:0$_pn:5%3Bexp-session$_st:1481919041905$ses_id:1481914837673%3Bexp-session$dc_visit:1$dc_event:2%3Bexp-session$dc_region:eu-central-1%3Bexp-session; rlt_marketing_code_cookie={}; _ga=GA1.2.1370767347.1481914841; _gat_ua=1; _tq_id.TV-721872-1.19dc=757f75b03184cc01.1481914841.0.1481917242..; JSESSIONID=4F9A7F3AC132A604BA6DBF5705065D46; linfo=v.4,|0|0|255|1|0||||||||1040|0|0||0|0|0|-1|-1; user=; minfo=; accttype=; s_ppv=page.Car-Search%2C10%2C8%2C702%2C1304%2C702%2C1366%2C768%2C1%2CP; HMS=C6AEBE3E-6CE2-4481-9531-308DA3101B92; HSEWC=0");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();


                return ReadResponse(response);

            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)we.Response;
                //else return false;
            }
            catch (Exception e)
            {
                if (response != null) response.Close();
                //return false;
            }
            Console.WriteLine("--<RequestFinished");
            return null;
        }

        public string RemoveSpecialCharacters(string s)
        {
            Console.WriteLine("-->RemoveSpecialCharactersStarted");
            //StringBuilder sb = new StringBuilder();
            string sb = string.Empty;
            try
            {
                if (s != null)
                {
                    int len = s.Length;
                    char[] s2 = new char[len];
                    int i2 = 0;
                    for (int i = 0; i < len; i++)
                    {
                        char c = s[i];
                        if (c != '\r' && c != '\n' && c != '\t')
                            s2[i2++] = c;

                    }
                    sb = new String(s2, 0, i2);
                }
            }
            catch (Exception e)
            {
                Console.Write("EXCEPTION");

            }

            sb = sb.Replace(@"\", "");

            Console.WriteLine("--<RemoveSpecialCharactersFinished");
            return sb.ToString();
        }

        public string ReadResponse(HttpWebResponse response)
        {
            Console.WriteLine("-->ReadResponseStarted");
            using (Stream responseStream = response.GetResponseStream())
            {
                Stream streamToRead = responseStream;
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
                }

                using (StreamReader streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                {
                    Console.WriteLine("--<ReadResponse Finished ");
                    return streamReader.ReadToEnd();
                }
            }
        }

        public List<JOffer> GetAtlassOffers(string siteSource)
        {
            Console.WriteLine("-->GetOfferCount Started");
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(siteSource);
                Console.WriteLine("--<GetOffer attr Started");
                HtmlNodeCollection offersFound = doc.DocumentNode.SelectNodes(".//div[@data-price]");

                //HtmlNode offerFound = doc.DocumentNode.SelectNodes(".//div[@data-price]");

                List<JOffer> offers = new List<JOffer>();
                if (offersFound != null)
                {
                    foreach (HtmlNode divNode in offersFound)
                    {
                        HtmlAttribute price = divNode.Attributes["data-price"];
                        HtmlAttribute category = divNode.Attributes["data-acriss"];
                        //HtmlAttribute supplier = supplierInfo.Attributes["data-acriss"];
                        string supplier = divNode.SelectNodes(".//div[@class='overlay']//ul//li//a").ElementAt(2).Attributes["href"].Value.Split('/').Last();

                        JOffer offer = new JOffer(Atlass.GetSupplier(supplier), price.Value);

                        offer.SetCategory(category.Value);
                        offers.Add(offer);
                    }
                    Console.WriteLine("--<GetOffer attr Finished");
                    return offers;
                }
                else
                {
                    return offers;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Offers not found");
                return null;
            }
        }
    }
}
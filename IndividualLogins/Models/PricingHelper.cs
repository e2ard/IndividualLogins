using IndividualLogins.Controllers.App_Code;
using IndividualLogins.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace IndividualLogins.Models
{
    class PricingHelper
    {
        PricingModel Sf;
        Dictionary<string, float> brokerDiscounts;
        PricingClass[] Classes;
        public PricingHelper(PricingModel sf, PricingClass[] classes)
        {
            Sf = sf;
            Classes = classes;
            SetBrokerDiscounts();
        }

        private string GetBrokerName(int? source)
        {
            switch (source)
            {
                case 1:
                    return "JIG";
                case 2:
                    return "CTR";
                case 3:
                    return "CTR";
            }
            return null;
        }
        public string SetDates()
        {
            Dictionary<string, Dictionary<string, JOffer>> pdfRates = null;
            string brokerName = GetBrokerName(Sf.Location);
            SiteBase site = null;

            Rates rates = new Rates();
            pdfRates = rates.GetRates(Sf, out site);
            
            for (int i = 0; i < Classes.Count(); i++)
            {
                List<JOffer> categoryOffers = GetMiniRatesList(pdfRates, Classes[i].ClassName);
                CsvHelper csvHelper = new CsvHelper();

                brokerName = "JIG";
                List<float> priceList = InitiatePrices(categoryOffers, brokerName, Classes[i].ClassName);
                string body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, brokerName, priceList);
                
                brokerName = "CTR";
                priceList = InitiatePrices(categoryOffers, brokerName, Classes[i].ClassName);
                body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, brokerName, priceList, body);

                Thread.Sleep(800);

                string responseText = Request_gmlithuania_fusemetrix_com(Classes[i].ClassLinkId, body);
                if (responseText.Length < 50)
                    throw new Exception("Something went wrong");

                Thread.Sleep(1000);
            }
            return rates.CreatePdf(site, pdfRates);
        }

        public string Excecute()
        {
            Dictionary<string, Dictionary<string, JOffer>> pdfRates = null;
            string brokerName = GetBrokerName(Sf.Location);
            SiteBase site = null;

            Rates rates = new Rates();
            pdfRates = rates.GetRates(Sf, out site);

            if (pdfRates.Values.FirstOrDefault().Count > 0)
            {
                for (int i = 0; i < Classes.Count(); i++)
                {
                    List<JOffer> categoryOffers = GetMiniRatesList(pdfRates, Classes[i].ClassName);

                    CsvHelper csvHelper = new CsvHelper(Classes[i].ClassLinkId);

                    List<float> fuseRates = csvHelper.GetFuseRates(brokerName);
                    List<float> priceList = CalculatePrices(categoryOffers, fuseRates, brokerName);

                    string body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, brokerName, priceList);
                    List<string> brokers = csvHelper.GetFuseBrokers();

                    foreach (string broker in brokers)//rewrite old rates possible unexpected results
                    {
                        if (Sf.ApplyToAll && !(broker.Equals("CTR") || broker.Equals("JIG")))
                            fuseRates = priceList.Select(s => s * BrokerDiscount(broker)).ToList();
                        else
                            fuseRates = csvHelper.GetFuseRates(broker);
                        body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, broker, fuseRates, body);
                    }

                    body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, brokerName, priceList, body);

                    Thread.Sleep(800);

                    string responseText = Request_gmlithuania_fusemetrix_com(Classes[i].ClassLinkId, body);
                    if (responseText.Length < 50)
                        throw new Exception("Something went wrong");

                    Thread.Sleep(800);
                }
            }
            return rates.CreatePdf(site, pdfRates);
        }

        private List<float> CalculatePrices(List<JOffer> categoryOffers, List<float> fuseRates, string brokerName)
        {
            if (fuseRates.Count() == 0)
                return null;

            List<float> priceList = new List<float>();
            float lastAddedPrice = 0;
            float avg = GetAverage(categoryOffers.Select(s => s.price).ToList());


            for (int i = 0; i < 30; i++)
            {
                if (i < categoryOffers.Count())
                {
                    float gmPrice = categoryOffers.ElementAt(i).gmPrice;
                    float price = categoryOffers.ElementAt(i).price;
                    float fusePrice = fuseRates.ElementAt(i);
                    float priceDiff = price - gmPrice;
                    lastAddedPrice = CalculatePriceJIG(gmPrice, price, fusePrice);

                    if (brokerName.Equals("JIG"))
                        priceList.Add(!((categoryOffers.ElementAt(i).GetSupplier() != null? categoryOffers.ElementAt(i).GetSupplier().Contains("glob") : false) && priceDiff < 2.5f && priceDiff > 0)
                            ? lastAddedPrice
                            : fusePrice);

                    if (brokerName.Equals("CTR"))
                        priceList.Add(CalculatePriceCTR(gmPrice, price, fusePrice));
                }
                else
                {
                    //if (brokerName.Equals("JIG"))
                     priceList.Add(avg > 0? lastAddedPrice += avg: fuseRates.ElementAt(i));// if supplier price is 0

                    //if (brokerName.Equals("CTR"))
                    //    priceList.Add(CalculatePriceCTR(gmPrice, price, fusePrice));
                }
            }
            return priceList;
        }

        private List<float> InitiatePrices(List<JOffer> categoryOffers, string brokerName, string className)
        {
            List<float> priceList = new List<float>();
            float classCoef = 1;

            switch (className)
            {
                case "MiniM":
                    classCoef = 0.74f;
                    break;
                case "EconomyM":
                    classCoef = 0.72f;
                    break;
                case "EconomyA":
                    classCoef = 0.81f;
                    break;
                case "CompactM":
                    classCoef = 0.8f;
                    break;
                case "CompactA":
                    classCoef = 0.8f;
                    break;
            }


            float avg = GetAverage(categoryOffers.Select(s => s.price).ToList());
            float lastAddedPrice = 0;

            for (int i = 0; i < 30; i++)
            {
                if (i < categoryOffers.Count()){
                    if (brokerName.Equals("JIG"))
                    {
                        classCoef = categoryOffers.ElementAt(i).price > 100 ? classCoef += 0.01f : classCoef -= 0.01f;
                        lastAddedPrice = categoryOffers.ElementAt(i).price * classCoef - categoryOffers.ElementAt(i).price * classCoef / 10;
                        priceList.Add(lastAddedPrice);
                    }
                    if (brokerName.Equals("CTR"))
                    {
                        lastAddedPrice = categoryOffers.ElementAt(i).price * 0.95f;
                        priceList.Add(lastAddedPrice);
                    }
                }
                else
                    priceList.Add(lastAddedPrice += avg);
            }
            return priceList;
        }

        
        private float BrokerDiscount(string brokerName)
        {

            return brokerDiscounts.ContainsKey(brokerName) ? brokerDiscounts[brokerName] : 0;
        }

        private float GetAverage(List<float> supplierPrices)
        {
            float avgSum = 0;
            int sumNum = 0;

            for(int i = 1; i < supplierPrices.Count; i++)
            {
                if (supplierPrices.ElementAt(i) > 0 && supplierPrices.ElementAt(i - 1) > 0)
                {
                    avgSum += Math.Abs(supplierPrices.ElementAt(i) - supplierPrices.ElementAt(i - 1));
                    sumNum++;
                }
            }

            return avgSum / sumNum > 0? avgSum / sumNum: 14 ;
        }
        private void SetBrokerDiscounts()
        {
            brokerDiscounts = new Dictionary<string, float>();
            brokerDiscounts.Add("BASE", 1.25f);
            brokerDiscounts.Add("ABK", 1.125f);
            brokerDiscounts.Add("ACH", 1);
            brokerDiscounts.Add("AMG", 1);
            brokerDiscounts.Add("ATC", 1);
            brokerDiscounts.Add("ATL", 1);
            brokerDiscounts.Add("AES", 1);
            brokerDiscounts.Add("AEU", 1);
            brokerDiscounts.Add("BKG", 1);
            brokerDiscounts.Add("BMK", 1);
            brokerDiscounts.Add("BSP", 1);
            brokerDiscounts.Add("CDM", 1);
            brokerDiscounts.Add("CHL", 1);
            brokerDiscounts.Add("CRB", 1);
            brokerDiscounts.Add("CR8", 1);
            brokerDiscounts.Add("CRL", 1);
            brokerDiscounts.Add("CRR", 1);
            brokerDiscounts.Add("DTA", 1);
            brokerDiscounts.Add("ECH", 1);
            brokerDiscounts.Add("ECR", 1);
            brokerDiscounts.Add("ENJ", 1);
            brokerDiscounts.Add("EXP", 1);
            brokerDiscounts.Add("FXA", 1);
            brokerDiscounts.Add("GRC", 1);
            brokerDiscounts.Add("MMX", 1);
            brokerDiscounts.Add("NOV", 1);
            brokerDiscounts.Add("TUI", 1);
            brokerDiscounts.Add("RCG", 1);
            brokerDiscounts.Add("RIT", 1);
            brokerDiscounts.Add("SKY", 1);
            brokerDiscounts.Add("TIN", 1);
            brokerDiscounts.Add("VRT", 1);
            brokerDiscounts.Add("ZUC", 1.125F);

        }
        private static string ReadResponse(HttpWebResponse response)
        {
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
                    return streamReader.ReadToEnd();
                }
            }
        }

        public List<JOffer> GetMiniRatesList(Dictionary<string, Dictionary<string, JOffer>> rates, string categoryName)
        {
            List<JOffer> miniOffers = new List<JOffer>();
            foreach (string link in rates.Keys.ToList())
            {
                Dictionary<string, JOffer> map = rates[link];
                if (map.Count > 0)
                {
                    Category item = Const.categories.FirstOrDefault(f => f.Name.Equals(categoryName));
                    {
                        if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
                            miniOffers.Add(map[item.Name]);
                        else
                            miniOffers.Add(new JOffer());
                    }
                }
            }
            return miniOffers;
        }

        private string Request_gmlithuania_fusemetrix_com(string linkId, string body)
        {
            HttpWebResponse response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://gmlithuania.fusemetrix.com/bespoke/rate_manager/old_price_override/overrideprice_edit.php?id=" + linkId);

                request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
                request.Headers.Set(HttpRequestHeader.ProxyAuthorization, "Basic ZWR2YXJkLm5hdXNAZ21haWwuY29tOjQyMWMzNDIxYzM=");
                request.Headers.Add("Origin", @"http://gmlithuania.fusemetrix.com");
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Referer = "http://gmlithuania.fusemetrix.com/bespoke/rate_manager/old_price_override/overrideprice_edit.php?id=296333";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");
                request.Headers.Set(HttpRequestHeader.Cookie, @"PHPSESSID=v90b7mukt7d9t9c40tm0qb6fi3; mwoid=24; mwornd=327915990");


                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return null;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return null;
            }
            string responseText = ReadResponse(response);
            response.Close();
            return responseText;
        }
        private float CalculatePriceCTR(float gmPrice, float price, float fusePrice)
        {
            float overridePrice = price;
            float priceDiff = gmPrice - price;
            float amountCoef = price > 95 ? 0.8f : 1;


            if (priceDiff >= 0)
                overridePrice = fusePrice - priceDiff * gmPrice / fusePrice * amountCoef;
            else
                overridePrice = fusePrice - priceDiff * gmPrice / fusePrice * amountCoef;

            if (priceDiff < 0.7f && priceDiff > 0)
                return fusePrice - priceDiff;

            if ((price == 0 || gmPrice == 0) || (price - gmPrice < 1.5f) && price - gmPrice > 0)
                return fusePrice;

            if (overridePrice < price * 0.6f || fusePrice < 0 || gmPrice < fusePrice)
                return price * 0.96f;

            return overridePrice;
        }

        private float CalculatePriceJIG(float gmPrice, float price, float fusePrice)
        {
            if (price == 0)
                return fusePrice;

            float overridePrice = price;
            float priceDiff = gmPrice - price;

            if (priceDiff >= 0)
            {
                if (priceDiff < 20)
                {
                    if (priceDiff < 5)
                    {
                        if (priceDiff < 3)
                        {
                            if (priceDiff < 2)
                            {
                                if (priceDiff < 1)
                                {
                                    if (priceDiff == 0)
                                        overridePrice = fusePrice - 0.01f;//if price are equal
                                    else
                                        overridePrice = fusePrice - priceDiff * 0.97f;
                                }
                                else
                                    overridePrice = fusePrice - priceDiff * 0.94f;
                            }
                            else
                                overridePrice = fusePrice - priceDiff * 0.73f;
                        }
                        else
                            overridePrice = fusePrice - priceDiff * 0.8f;
                    }
                    else
                        overridePrice = fusePrice - priceDiff * 0.6f;
                }
                else
                    overridePrice = price * 0.75f - priceDiff * 0.3f;
            }
            else
            {
                if (Math.Abs(priceDiff) < 20)
                {
                    if (Math.Abs(priceDiff) < 3)
                        if (priceDiff > 1.5f)
                            overridePrice = fusePrice + Math.Abs(priceDiff) * 0.42f;
                        else
                            overridePrice = fusePrice + Math.Abs(priceDiff) * 0.25f;
                    else
                        overridePrice = fusePrice + Math.Abs(priceDiff) * 0.52f;
                    //overridePrice = fusePrice - priceDiff * gmPrice / fusePrice * 0.5f;
                }
                else
                    overridePrice = price * 0.8f;
            }


            if ((price == 0 || gmPrice == 0) || (price - gmPrice < 1.5f) && price - gmPrice > 0 || overridePrice < 0)
                return fusePrice;

            if (overridePrice < price * 0.67f || fusePrice < 0 || gmPrice < fusePrice || (gmPrice == 0 && price > 0))//attention (latest change)
                return price * 0.69f;

            return overridePrice;
        }

    }
}

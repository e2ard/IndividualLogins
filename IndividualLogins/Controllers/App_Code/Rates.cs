using IndividualLogins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using IndividualLogins.Models.NlogTest.Models;
using System.IO;
using OfficeOpenXml;
using static JOffer;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IndividualLogins.Controllers.App_Code
{
    public class Rates
    {
        public string GetPdfLocation(SiteBase s, SearchFilters sf)
        {
            Log.Instance.Info("---Begin: GetPdfLocation");
            Dictionary<string, Dictionary<string, JOffer>> offerMap = GetRates(sf, out s);

            string fileName;
            fileName = sf.IsPdf > 0? CreatePdf(s, offerMap): GetExcel(s, offerMap);

            Log.Instance.Info("---End: GetPdfLocation");
            return fileName;
        }

        public Dictionary<string, Dictionary<string, JOffer>> GetRates(SearchFilters searchFilters, out SiteBase tempSite)
        {
            Log.Instance.Warn("---Begin: GetRates");
            switch (searchFilters.Source)
            {
                case 1:
                    return GetRentalRates(searchFilters, out tempSite);
                case 2:
                    return GetCarTrawlerRates(searchFilters, out tempSite);
                case 3:
                    return GetScannerRates(searchFilters, out tempSite);
            }
            Log.Instance.Warn("---End: GetRates");
            tempSite = null;
            return null;
        }

        public string GetExcel(SiteBase s, Dictionary<string, Dictionary<string, JOffer>> offerMap)
        {
            FileInfo template = new FileInfo(HttpContext.Current.Server.MapPath(@"\Content\ExcelPackageTemplate.xlsx"));
            string filename = @"\excel\" + s.GetTitle() + s.GetPuMonth() + "-" + s.GetPuDay() + s.GetCity() + ".xlsx";
            FileInfo newFile = new FileInfo(HttpContext.Current.Server.MapPath(filename));

            using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
            {
                ExcelWorkbook myWorkbook = excelPackage.Workbook;// Getting the complete workbook...
                ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Rates"];// Getting the worksheet by its name...

                int rowNum = 2;
                DateTime doDate = new DateTime(Convert.ToInt32(s.GetPuYear()), Convert.ToInt32(s.GetPuMonth()), Convert.ToInt32(s.GetPuDay()));
                foreach (string link in offerMap.Keys.ToList())
                {
                    Dictionary<string, JOffer> map = offerMap[link];
                    List<JOffer> offers = OffersToArray(map, link);

                    myWorksheet.Cells[rowNum, 1].Value = s.GetPuMonth() + "-" + s.GetPuDay() + "/" + doDate.AddDays(rowNum - 1).Day + "\n" + (rowNum - 1);
                    for (int i = 0; i < offers.Count; i++)
                    {
                        myWorksheet.Row(rowNum).Height = 50;
                        myWorksheet.Row(rowNum + 1).Height = 50;
                        myWorksheet.Row(rowNum + 2).Height = 50;

                        JOffer o = offers.ElementAt(i);
                        SupplierNew other = o.GetOtherSupplier();
                        SupplierNew gm = o.GetGmSupplier();
                        SupplierNew cr = o.GetCrSupplier();

                        myWorksheet.Cells[rowNum, i + 2].Value = (gm != null ? gm.ToString() : "");
                        myWorksheet.Cells[rowNum + 1, i + 2].Value = (cr != null ? cr.ToString() : "");
                        myWorksheet.Cells[rowNum + 2, i + 2].Value = (other != null ? other.ToString() : "");
                    }
                    rowNum += 3;
                }
                excelPackage.Save();// Saving the change...
                return filename;
            }
        }

        public string CreatePdf(SiteBase s, Dictionary<string, Dictionary<string, JOffer>> offerMap)
        {
            PdfBuilder pdf = new PdfBuilder(s);
            pdf.CreateHeaders();

            foreach (string link in offerMap.Keys.ToList())
            {
                Dictionary<string, JOffer> map = offerMap[link];
                List<JOffer> offers = OffersToArray(map, link);
                pdf.AddRow(offers.ToArray());
            }
            pdf.Close();
            return pdf.fileName;
        }

        public List<JOffer> OffersToArray(Dictionary<string, JOffer> map, string link)
        {
            List<JOffer> offers = new List<JOffer>();
            if (map.Count > 0)
            {
                foreach (Category item in Const.categories)
                {
                    if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
                    {
                        map[item.Name].SetSiteName(link);
                        offers.Add(map[item.Name]);
                    }
                    else
                        offers.Add(new JOffer());
                }
            }
            return offers;
        }


        public Dictionary<string, Dictionary<string, JOffer>> GetRentalRates(SearchFilters searchFilters, out SiteBase site)
        {
            Log.Instance.Warn("---Begin: GetRentalRates");
            DateTime sDate = searchFilters.PuDate;
            DateTime eDate = searchFilters.DoDate;

            Rental s = new Rental(Const.Locations[searchFilters.Location].Rental);

            s.SetTime(searchFilters.PuTime.Hours, searchFilters.PuTime.Minutes, searchFilters.DoTime.Hours, searchFilters.DoTime.Minutes);
            s.InitDate(sDate);
            site = s;
            int numOfIterations = (eDate - sDate).Days;

            List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
            List<JOffer> minOffers = new List<JOffer>();

            Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();
            for (int i = 0; i < links.Count; i++)
                offerMap.Add(links[i], new Dictionary<string, JOffer>());

            Func<object, int> action = (object obj) =>
            {
                int i = (int)obj;
                JSourceReader reader = new JSourceReader();
                offerMap[links.ElementAt(i)] = reader.GetMap(reader.ExtractOffers(reader.GetResultGroup(links.ElementAt(i))));
                return 0;
            };
            RunThreads(action, links.Count);
            Log.Instance.Warn("---END: GetRentalRates");
            return offerMap;
        }

        public Dictionary<string, Dictionary<string, JOffer>> GetCarTrawlerRates(SearchFilters searchFilters, out SiteBase site)
        {
            DateTime stime = DateTime.Now;
            Trawler s = new Trawler(Const.Locations[searchFilters.Location].CarTrawler);
            DateTime sDate = searchFilters.PuDate;//.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
            DateTime eDate = searchFilters.DoDate;//.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);
            s.InitDate(sDate);

            int numOfIterations = (eDate - sDate).Days;
            List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
            site = s;

            Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

            for (int i = 0; i < links.Count; i++)
                offerMap.Add(links[i], new Dictionary<string, JOffer>());

            Func<object, int> action = (object obj) =>
            {
                int i = (int)obj;
                JSourceReader reader = new JSourceReader();
                offerMap[links.ElementAt(i)] = reader.GetMap(reader.GetNorwRates(links.ElementAt(i)));
                return 0;
            };

            RunThreads(action, links.Count);
            Debug.WriteLine("Time elapsed" + (DateTime.Now - stime).Seconds);
            return offerMap;
        }

        public Dictionary<string, Dictionary<string, JOffer>> GetCarTrawlerRatesSingle(SearchFilters searchFilters, out SiteBase site)
        {
            Trawler s = new Trawler(Const.Locations[searchFilters.Location].CarTrawler);
            DateTime sDate = searchFilters.PuDate;//.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
            DateTime eDate = searchFilters.DoDate;//.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);
            s.InitDate(sDate);

            int numOfIterations = (eDate - sDate).Days;

            List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
            site = s;

            List<JOffer> minOffers = new List<JOffer>();

            Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

            for (int i = 0; i < links.Count; i++)
                offerMap.Add(links[i], new Dictionary<string, JOffer>());


            List<Thread> threads = new List<Thread>();
            //--- Start all threads
            for (int index = 0; index < links.Count; index++)
            {
                JSourceReader reader = new JSourceReader();
                offerMap[links.ElementAt(index)] =
                        reader.GetMap(reader.GetNorwRates(links.ElementAt(index)));
            }

            return offerMap;
        }

        public Dictionary<string, Dictionary<string, JOffer>> GetScannerRates(SearchFilters searchFilters, out SiteBase site)
        {
            Trawler s = new Trawler(Const.Locations[searchFilters.Location].CarScanner);
            DateTime sDate = searchFilters.PuDate.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
            DateTime eDate = searchFilters.DoDate.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);
            s.InitDate(sDate);

            int numOfIterations = (eDate - sDate).Days;

            List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
            List<JOffer> minOffers = new List<JOffer>();

            Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

            for (int i = 0; i < links.Count; i++)
                offerMap.Add(links[i], new Dictionary<string, JOffer>());


            Func<object, int> action = (object obj) =>
            {
                int i = (int)obj;
                JSourceReader reader = new JSourceReader();
                offerMap[links.ElementAt(i)] = reader.GetMap(reader.GetNorwRates(links.ElementAt(i)));
                return 0;
            };
            RunThreads(action, links.Count);
            s.SetTitle("scanner");
            site = s;
            return offerMap;
        }

        public void RunThreads(Func<object, int> action, int linkCount)
        {
            var tasks = new List<Task<int>>();
            for (int index = 0; index < linkCount; index++)
            {
                tasks.Add(Task<int>.Factory.StartNew(action, index));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
            }
        }
    }
}
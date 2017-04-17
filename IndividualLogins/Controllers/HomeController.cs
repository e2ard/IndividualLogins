using IndividualLogins.Controllers.App_Code;
using IndividualLogins.Models;
using IndividualLogins.Models.Dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using IndividualLogins.Controllers.App_Code;

namespace IndividualLogins.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        PricingToolDal dal = new PricingToolDal();
        [Authorize(Roles = "Admin, Edit, Preview")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Edit") || User.IsInRole("Preview"))
            {
                ViewBag.Locations = GetLocations();
                ViewBag.Sources = dal.GetSources();
                return View(new SearchFilters());
            }
            else
                return RedirectToAction("Login", "Account");
        }

        //public ActionResult Index()
        //{
        //    ViewBag.Locations = GetLocations();
        //    ViewBag.Sources = dal.GetSources();
        //    return View(new SearchFilters());
        //}

        //[HttpPost]
        //public ActionResult Index(SearchFilters searchFilters)
        //{
        //    ViewBag.Locations = dal.GetLocations();
        //    ViewBag.Sources = dal.GetSources();
        //    ViewBag.Countries = dal.GetCoutries();
        //    string fileName = "";
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            switch (searchFilters.Source)
        //            {
        //                case 1:
        //                    fileName = GetRentalPdf(searchFilters);
        //                    break;
        //                case 2:
        //                    fileName = GetCarTrawlerPdf(searchFilters);
        //                    break;
        //                case 3:
        //                    fileName = GetScannerPdf(searchFilters);
        //                    break;
        //                //case 4:
        //                //    fileName = GetEconomyPdf(searchFilters);
        //                //    break;
        //            }
        //            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/" + fileName));
        //            ViewBag.Message = "Done";
        //            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName.Substring(5, fileName.Length - 5));
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        //Debug.WriteLine(exp.InnerException);
        //        //Response.Write("<script>alert('Proxy Error');</script>");
        //    }
        //    return View(searchFilters);
        //}

        public string GetResultFileName(SearchFilters searchFilters)
        {
            string fileName = "";
            SiteBase site = null;
            Dictionary<string, Dictionary<string, JOffer>> rates = null;
            if (ModelState.IsValid)
            {
                switch (searchFilters.Source)
                {
                    case 1:
                        rates = GetRentalPdf(searchFilters, out site);
                        break;
                    case 2:
                        rates = GetCarTrawlerPdf(searchFilters, out site);
                        break;
                    case 3:
                        rates = GetScannerPdf(searchFilters, out site);
                        break;
                }
                return fileName = CreatePdf(site, rates);
            }
            else
                return "";
        }

        //public string GetRentalExcel(SearchFilters searchFilters)
        //{
        //    DateTime sDate = (DateTime)searchFilters.PuDate;
        //    DateTime eDate = searchFilters.DoDate;

        //    Rental s = new Rental(Const.Locations[searchFilters.Location].Rental);

        //    s.SetTime(searchFilters.PuTime.Hours, searchFilters.PuTime.Minutes, searchFilters.DoTime.Hours, searchFilters.DoTime.Minutes);
        //    s.InitDate(sDate);

        //    int numOfIterations = (eDate - sDate).Days;

        //    List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
        //    List<JOffer> minOffers = new List<JOffer>();

        //    Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();
        //    for (int i = 0; i < links.Count; i++)
        //        offerMap.Add(links[i], new Dictionary<string, JOffer>());

        //    List<Thread> threads = new List<Thread>();
        //    //--- Start all threads
        //    for (int index = 0; index < links.Count; index++)
        //    {
        //        Thread thread = new Thread(() =>
        //        {
        //            JSourceReader reader = new JSourceReader();
        //            offerMap[Thread.CurrentThread.Name == null ? links.ElementAt(0) : Thread.CurrentThread.Name] =
        //            reader.GetMap(reader.ExtractOffers(reader.GetResultGroup(Thread.CurrentThread.Name)));
        //        });
        //        thread.Name = links.ElementAt(index);
        //        threads.Add(thread);
        //        thread.Start();
        //    }

        //    //check if thread has done
        //    Boolean allCompleted = false;
        //    while (!allCompleted)
        //    {
        //        int completed = links.Count;
        //        for (int i = 0; i < links.Count; i++)
        //        {
        //            if (!threads.ElementAt(i).IsAlive)
        //                --completed;
        //            else
        //            {
        //                Thread.Sleep(300);
        //                break;
        //            }
        //        }
        //        if (completed == 0)
        //            break;
        //    }
        //    FileInfo template = new FileInfo(Server.MapPath(@"\Content\ExcelPackageTemplate.xlsx"));
        //    string filename = @"\excel\" + s.GetTitle() + s.GetPuMonth() + "-" + s.GetPuDay() + s.GetCity() + ".xlsx";
        //    FileInfo newFile = new FileInfo(Server.MapPath(filename));

        //    using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
        //    {
        //        // Getting the complete workbook...
        //        ExcelWorkbook myWorkbook = excelPackage.Workbook;

        //        // Getting the worksheet by its name...
        //        ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Rates"];
        //        int rowNum = 2;
        //        DateTime doDate = new DateTime(Convert.ToInt32(s.GetPuYear()), Convert.ToInt32(s.GetPuMonth()), Convert.ToInt32(s.GetPuDay()));
        //        foreach (string link in links)
        //        {
        //            Dictionary<string, JOffer> map = offerMap[link];
        //            List<JOffer> offers = new List<JOffer>();
        //            if (map.Count > 0)
        //            {
        //                foreach (Category item in Const.categories)
        //                {
        //                    if (map.ContainsKey(item.Name))
        //                    {
        //                        if (map[item.Name] != null)
        //                        {
        //                            map[item.Name].SetSiteName(link);
        //                            offers.Add(map[item.Name]);
        //                        }
        //                    }
        //                    else
        //                        offers.Add(new JOffer());
        //                }
        //            }
        //            else
        //                System.Diagnostics.Debug.WriteLine("Map count 0");

        //            myWorksheet.Cells[rowNum, 1].Value = s.GetPuMonth() + "-" + s.GetPuDay() + "/" + doDate.AddDays(rowNum - 1).Day + "\n" + (rowNum - 1);
        //            for (int i = 0; i < offers.Count; i++)
        //            {
        //                myWorksheet.Cells[rowNum, i + 2].Value = offers.ElementAt(i).GetOffer();
        //                myWorksheet.Row(rowNum).Height = 35;
        //            }
        //            ++rowNum;
        //        }
        //        excelPackage.Save();// Saving the change...
        //        return filename;
        //    }
        //}

        //public string GetExcelFileName(SearchFilters searchFilters)
        //{
        //    string fileName = "";

        //    if (ModelState.IsValid)
        //    {
        //        switch (searchFilters.Source)
        //        {
        //            case 1:
        //                fileName = GetRentalExcel(searchFilters);
        //                break;
        //            case 2:
        //                fileName = GetCarTrawlerExcel(searchFilters);
        //                break;
        //            case 3:
        //                fileName = GetScannerExcel(searchFilters);
        //                break;
        //            //case 4:
        //            //    fileName = GetEconomyPdf(searchFilters);
        //            //    break;
        //            //default:
        //            //    fileName = GetRentalExcel(searchFilters);
        //            //    break;
        //        }
        //        return fileName;
        //    }
        //    else
        //        return "";
        //}

        public Dictionary<string, Dictionary<string, JOffer>> GetRentalPdf(SearchFilters searchFilters, out SiteBase site)
        {
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

            List<Thread> threads = new List<Thread>();
            //--- Start all threads
            for (int index = 0; index < links.Count; index++)
            {
                Thread thread = new Thread(() =>
                {
                    JSourceReader reader = new JSourceReader();
                    offerMap[Thread.CurrentThread.Name == null ? links.ElementAt(0) : Thread.CurrentThread.Name] =
                    reader.GetMap(reader.ExtractOffers(reader.GetResultGroup(Thread.CurrentThread.Name)));
                });
                thread.Name = links.ElementAt(index);
                threads.Add(thread);
                thread.Start();
            }

            //check if thread has done
            Boolean allCompleted = false;
            while (!allCompleted)
            {
                int completed = links.Count;
                for (int i = 0; i < links.Count; i++)
                {
                    if (!threads.ElementAt(i).IsAlive)
                        --completed;
                    else
                    {
                        Thread.Sleep(300);
                        break;
                    }
                }
                if (completed == 0)
                    break;
            }
            return offerMap;
        }

        public Dictionary<string, Dictionary<string, JOffer>> GetCarTrawlerPdf(SearchFilters searchFilters, out SiteBase site)
        {
            Trawler s = new Trawler(Const.Locations[searchFilters.Location].CarTrawler);
            DateTime sDate = searchFilters.PuDate.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
            DateTime eDate = searchFilters.DoDate.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);
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
                Thread thread = new Thread(() =>
                {
                    JSourceReader reader = new JSourceReader();
                    offerMap[Thread.CurrentThread.Name == null ?
                        links.ElementAt(0) :
                        Thread.CurrentThread.Name] =
                            reader.GetMapNorwegian(reader.GetNorwRates(Thread.CurrentThread.Name));
                });
                thread.Name = links.ElementAt(index);
                threads.Add(thread);
                thread.Start();
            }

            //check if threads has done
            Boolean allCompleted = false;
            while (!allCompleted)
            {
                int completed = links.Count;
                for (int i = 0; i < links.Count; i++)
                {
                    if (!threads.ElementAt(i).IsAlive)
                        --completed;
                    else
                    {
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (completed == 0)
                    break;
            }

            return offerMap;
        }

        //public string GetVehicleRentPdf(SearchFilters searchFilters)
        //{
        //    DateTime sDate = searchFilters.PuDate.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
        //    DateTime eDate = searchFilters.DoDate.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);

        //    Vehicle s = new Vehicle(Const.Locations[searchFilters.Location].EcoBoking);
        //    s.InitDate(sDate);

        //    int numOfIterations = (eDate - sDate).Days;

        //    List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
        //    List<JOffer> minOffers = new List<JOffer>();

        //    Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

        //    for (int i = 0; i < links.Count; i++)
        //        offerMap.Add(links[i], new Dictionary<string, JOffer>());


        //    for (int i = 0; i < links.Count; i++)
        //    {
        //        JSourceReader reader = new JSourceReader();
        //        List<JOffer> offers = reader.GetVehicleOffers(
        //                            reader.GetVehicleSource(links.ElementAt(i)));

        //        offerMap[links.ElementAt(i)] =
        //                reader.GetMapNorwegian(offers);

        //    }

        //    return CreatePdf(s, offerMap);
        //}

        //public string GetCarTrawlerExcel(SearchFilters searchFilters)
        //{
        //    DateTime sDate = searchFilters.PuDate.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
        //    DateTime eDate = searchFilters.DoDate.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);

        //    Trawler s = new Trawler(Const.Locations[searchFilters.Location].CarTrawler);
        //    s.InitDate(sDate);

        //    int numOfIterations = (eDate - sDate).Days;

        //    List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
        //    List<JOffer> minOffers = new List<JOffer>();

        //    Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

        //    for (int i = 0; i < links.Count; i++)
        //        offerMap.Add(links[i], new Dictionary<string, JOffer>());

        //    List<Thread> threads = new List<Thread>();
        //    for (int index = 0; index < links.Count; index++)//--- Start all threads
        //    {
        //        Thread thread = new Thread(() =>
        //        {
        //            JSourceReader reader = new JSourceReader();
        //            offerMap[Thread.CurrentThread.Name == null ?
        //                links.ElementAt(0) :
        //                Thread.CurrentThread.Name] =
        //                    reader.GetMapNorwegian(reader.GetNorwRates(Thread.CurrentThread.Name));
        //        });
        //        thread.Name = links.ElementAt(index);
        //        threads.Add(thread);
        //        thread.Start();
        //    }

        //    Boolean allCompleted = false;//check if threads has done
        //    while (!allCompleted)
        //    {
        //        int completed = links.Count;
        //        for (int i = 0; i < links.Count; i++)
        //        {
        //            if (!threads.ElementAt(i).IsAlive)
        //                --completed;
        //            else
        //            {
        //                Thread.Sleep(100);
        //                break;
        //            }
        //        }
        //        if (completed == 0)
        //            break;
        //    }

        //    FileInfo template = new FileInfo(Server.MapPath(@"\Content\ExcelPackageTemplate.xlsx"));
        //    string filename = @"\excel\" + s.GetTitle() + s.GetPuMonth() + "-" + s.GetPuDay() + s.GetCity() + ".xlsx";
        //    FileInfo newFile = new FileInfo(Server.MapPath(filename));

        //    using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
        //    {
        //        ExcelWorkbook myWorkbook = excelPackage.Workbook;// Getting the complete workbook...
        //        ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Rates"];// Getting the worksheet by its name...

        //        int rowNum = 2;
        //        DateTime doDate = new DateTime(Convert.ToInt32(s.GetPuYear()), Convert.ToInt32(s.GetPuMonth()), Convert.ToInt32(s.GetPuDay()));

        //        foreach (string link in links)
        //        {
        //            Dictionary<string, JOffer> map = offerMap[link];
        //            List<JOffer> offers = new List<JOffer>();
        //            if (map.Count > 0)
        //            {
        //                foreach (Category item in Const.categories)
        //                {
        //                    if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
        //                    {
        //                        map[item.Name].SetSiteName(link);
        //                        offers.Add(map[item.Name]);
        //                    }
        //                    else
        //                        offers.Add(new JOffer());
        //                }
        //            }
        //            myWorksheet.Cells[rowNum, 1].Value = s.GetPuMonth() + "-" + s.GetPuDay() + "/" + doDate.AddDays(rowNum - 1).Day + "\n" + (rowNum - 1);
        //            for (int i = 0; i < offers.Count; i++)
        //            {
        //                myWorksheet.Cells[rowNum, i + 2].Value = offers.ElementAt(i).GetOffer();
        //                myWorksheet.Row(rowNum).Height = 45;
        //            }
        //            ++rowNum;
        //        }
        //        excelPackage.Save();// Saving the change...
        //    }
        //    return filename;
        //}

        public Dictionary<string, Dictionary<string, JOffer>> GetScannerPdf(SearchFilters searchFilters, out SiteBase site)
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


            List<Thread> threads = new List<Thread>();
            //--- Start all threads
            for (int index = 0; index < links.Count; index++)
            {
                Thread thread = new Thread(() =>
                {
                    JSourceReader reader = new JSourceReader();
                    offerMap[Thread.CurrentThread.Name == null ?
                        links.ElementAt(0) :
                        Thread.CurrentThread.Name] =
                            reader.GetMapNorwegian(reader.GetScannerRates(Thread.CurrentThread.Name));
                });
                thread.Name = links.ElementAt(index);
                threads.Add(thread);
                thread.Start();
            }

            //check if threads has done
            Boolean allCompleted = false;
            while (!allCompleted)
            {
                int completed = links.Count;
                for (int i = 0; i < links.Count; i++)
                {
                    if (!threads.ElementAt(i).IsAlive)
                        --completed;
                    else
                    {
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (completed == 0)
                    break;
            }
            s.SetTitle("scanner");
            site = s;
            return offerMap;
        }

        //public string GetScannerExcel(SearchFilters searchFilters)
        //{
        //    Scanner s = new Scanner(Const.Locations[searchFilters.Location].CarScanner);
        //    DateTime sDate = searchFilters.PuDate.AddHours(searchFilters.PuTime.Hours).AddMinutes(searchFilters.PuTime.Minutes);
        //    DateTime eDate = searchFilters.DoDate.AddHours(searchFilters.DoTime.Hours).AddMinutes(searchFilters.DoTime.Minutes);

        //    s.InitDate(sDate);
        //    int numOfIterations = (eDate - sDate).Days;

        //    List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
        //    List<JOffer> minOffers = new List<JOffer>();
        //    Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

        //    for (int i = 0; i < links.Count; i++)
        //        offerMap.Add(links[i], new Dictionary<string, JOffer>());

        //    List<Thread> threads = new List<Thread>();
        //    for (int index = 0; index < links.Count; index++)//--- Start all threads

        //    {
        //        Thread thread = new Thread(() =>
        //        {
        //            JSourceReader reader = new JSourceReader();
        //            offerMap[Thread.CurrentThread.Name == null ?
        //                links.ElementAt(0) :
        //                Thread.CurrentThread.Name] =
        //                    reader.GetMapNorwegian(reader.GetScannerRates(Thread.CurrentThread.Name));
        //        });
        //        thread.Name = links.ElementAt(index);
        //        threads.Add(thread);
        //        thread.Start();
        //    }

        //    //check if threads has done//--- Start all threads
        //    bool allCompleted = false;
        //    while (!allCompleted)
        //    {
        //        int completed = links.Count;
        //        for (int i = 0; i < links.Count; i++)
        //        {
        //            if (!threads.ElementAt(i).IsAlive)
        //                --completed;
        //            else
        //            {
        //                Thread.Sleep(100);
        //                break;
        //            }
        //        }
        //        if (completed == 0)
        //            break;
        //    }


        //    FileInfo template = new FileInfo(Server.MapPath(@"\Content\ExcelPackageTemplate.xlsx"));
        //    string filename = @"\excel\" + s.GetTitle() + s.GetPuMonth() + "-" + s.GetPuDay() + s.GetCity() + ".xlsx";
        //    FileInfo newFile = new FileInfo(Server.MapPath(filename));

        //    using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
        //    {
        //        ExcelWorkbook myWorkbook = excelPackage.Workbook;// Getting the complete workbook...
        //        ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Rates"];// Getting the worksheet by its name...

        //        int rowNum = 2;
        //        DateTime doDate = new DateTime(Convert.ToInt32(s.GetPuYear()), Convert.ToInt32(s.GetPuMonth()), Convert.ToInt32(s.GetPuDay()));

        //        foreach (string link in links)
        //        {
        //            Dictionary<string, JOffer> map = offerMap[link];
        //            List<JOffer> offers = new List<JOffer>();
        //            if (map.Count > 0)
        //            {
        //                foreach (Category item in Const.categories)
        //                {
        //                    if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
        //                    {
        //                        map[item.Name].SetSiteName(link);
        //                        offers.Add(map[item.Name]);
        //                    }
        //                    else
        //                        offers.Add(new JOffer());
        //                }
        //            }
        //            myWorksheet.Cells[rowNum, 1].Value = s.GetPuMonth() + "-" + s.GetPuDay() + "/" + doDate.AddDays(rowNum - 1).Day + "\n" + (rowNum - 1);
        //            for (int i = 0; i < offers.Count; i++)
        //            {
        //                myWorksheet.Cells[rowNum, i + 2].Value = offers.ElementAt(i).GetOffer();
        //                myWorksheet.Row(rowNum).Height = 45;
        //            }
        //            ++rowNum;
        //        }
        //        excelPackage.Save();// Saving the change...
        //    }
        //    return filename;
        //}

        //public string GetEconomyPdf(SearchFilters searchFilters)
        //{
        //    DateTime sDate = searchFilters.PuDate;
        //    DateTime eDate = searchFilters.DoDate;
        //    EcoBookings s = new EcoBookings(Const.Locations[searchFilters.Location].EcoBoking);
        //    JSourceReader s1 = new JSourceReader();
        //    sDate = sDate.AddDays(1);
        //    eDate = eDate.AddDays(1);
        //    s.InitDate(sDate);

        //    int numOfIterations = (eDate - sDate).Days;

        //    List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
        //    List<JOffer> minOffers = new List<JOffer>();
        //    Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

        //    for (int i = 0; i < links.Count; i++)
        //        offerMap.Add(links[i], new Dictionary<string, JOffer>());

        //    List<Thread> threads = new List<Thread>();
        //    for (int i = 0; i < links.Count; i++)
        //    {
        //        JSourceReader reader = new JSourceReader();
        //        List<JOffer> offers = reader.GetBookingOffers(
        //                            reader.GetBookingsSource(links.ElementAt(i), links.ElementAt(i)));

        //        offerMap[links.ElementAt(i)] =
        //                reader.GetMapNorwegian(offers);

        //    }
        //    return CreatePdf(s, offerMap);
        //}

        //public string GetExpediaPdf(SearchFilters searchFilters)
        //{
        //    DateTime sDate = searchFilters.PuDate;
        //    DateTime eDate = searchFilters.DoDate;
        //    Expedia s = new Expedia(Const.Locations[searchFilters.Location].EcoBoking);
        //    JSourceReader s1 = new JSourceReader();
        //    sDate = sDate.AddDays(1);
        //    eDate = eDate.AddDays(1);
        //    s.InitDate(sDate);

        //    int numOfIterations = (eDate - sDate).Days;

        //    List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
        //    List<JOffer> minOffers = new List<JOffer>();
        //    Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

        //    for (int i = 0; i < links.Count; i++)
        //        offerMap.Add(links[i], new Dictionary<string, JOffer>());

        //    List<Thread> threads = new List<Thread>();
        //    for (int i = 0; i < links.Count; i++)
        //    {
        //        JSourceReader reader = new JSourceReader();
        //        List<JOffer> offers = reader.GetExpediaOffers(
        //                            reader.GetExpediaSource(links.ElementAt(i)));

        //        offerMap[links.ElementAt(i)] =
        //                reader.GetMapNorwegian(offers);

        //    }
        //    return CreatePdf(s, offerMap);
        //}

        public string CreatePdf(SiteBase s, Dictionary<string, Dictionary<string, JOffer>> offerMap)
        {
            PdfBuilder pdf = new PdfBuilder(s);
            pdf.CreateHeaders();

            foreach (string link in offerMap.Keys.ToList())
            {
                Dictionary<string, JOffer> map = offerMap[link];
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
                pdf.addRow(offers.ToArray());
            }
            pdf.Close();
            return pdf.fileName;

        }

        public ActionResult News()
        {
            return View();
        }

        public IEnumerable<SelectListItem> GetLocations()
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            //sl.Add(new SelectListItem { Selected = false, Text = "Gdansk", Value = "12" });
            //sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Krakow", Value = "11" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Chopin)", Value = "4" });
            sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Modlin)", Value = "9" });
            //sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Fiumicino", Value = "6" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Rome", Value = "7" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Bologna", Value = "8" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Prague", Value = "10" });

            return new SelectList(sl, "Value", "Text");
        }

        public JsonResult GetLocations(int? country)
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            switch (country)
            {
                case 1:
                    sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
                    break;
                case 2:
                    sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
                    break;
                case 3:
                    sl.Add(new SelectListItem { Selected = false, Text = "Gdansk", Value = "12" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Krakow", Value = "11" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Chopin)", Value = "4" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Modlin)", Value = "9" });
                    break;
                case 4:
                    sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
                    break;
                case 5:
                    sl.Add(new SelectListItem { Selected = false, Text = "Fiumicino", Value = "6" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Rome", Value = "7" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Bologna", Value = "8" });
                    break;
                case 6:
                    sl.Add(new SelectListItem { Selected = false, Text = "Prague", Value = "10" });
                    break;
                default:
                    break;
            }
            IEnumerable<SelectListItem> data = new SelectList(sl, "Value", "Text");

            return new JsonResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
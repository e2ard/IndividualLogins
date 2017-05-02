using IndividualLogins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace IndividualLogins.Controllers.App_Code
{
    public class Rates
    {
        public string GetPdfLocation(SiteBase s, SearchFilters sf)
        {
            return CreatePdf(s, GetRates(sf, out s));
        }
        public Dictionary<string, Dictionary<string, JOffer>> GetRates(SearchFilters searchFilters, out SiteBase tempSite)
        {
            SiteBase site = null;
            switch (searchFilters.Source)
            {
                case 1:
                    return GetRentalRates(searchFilters, out tempSite);
                case 2:
                    return GetCarTrawlerRates(searchFilters, out tempSite);
                case 3:
                    return GetScannerRates(searchFilters, out tempSite);
            }
            tempSite = null;
            return null;
        }

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

        public Dictionary<string, Dictionary<string, JOffer>> GetRentalRates(SearchFilters searchFilters, out SiteBase site)
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

        public Dictionary<string, Dictionary<string, JOffer>> GetCarTrawlerRates(SearchFilters searchFilters, out SiteBase site)
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
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using IndividualLogins.Models.NlogTest.Models;
using static JOffer;

namespace IndividualLogins.Controllers.App_Code
{
    public class PdfBuilder : IDisposable
    {
        private PdfPTable table;
        private string documentTitle, city;
        public string PATH, fileName;
        private int puYear, puMonth, puDay;
        public SiteBase SiteName;
        private int dayNum;
        private Document doc;
        private DateTime doDate;
        private Font font = FontFactory.GetFont("Arial", 8, Font.NORMAL);
        private string suppliers = string.Empty;

        public PdfBuilder(SiteBase site)
        {
            if (site != null)
            {
                this.SiteName = site;
                SetDocumentDetails();

                PATH = HttpContext.Current.Server.MapPath("~/");
                fileName = "\\pdf\\" + documentTitle + puMonth + "-" + puDay + city + DateTime.Now.ToString("hhmm");
                FileStream fs;
                try
                {
                    fs = new FileStream(PATH + fileName + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                }
                catch(IOException ioe)
                {
                    fs = new FileStream(PATH +  fileName + "(1).pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                    Log.Instance.Warn("--- PdfBuilder" + ioe.Message);
                }
                fileName += ".pdf";
                doc = new Document(PageSize.A4, 10, 10, 10, 10);
                doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                doDate = new DateTime(puYear, puMonth, puDay);
                dayNum = 1;
                AddSuppliers("GREEN MOTION");
            }
            else
            {
                Log.Instance.Warn("--- PdfBuilder Site is null");
                throw new Exception("Site is null");
            }
        }
        public PdfBuilder()
        {
        }

        public void Close()
        {
            doc.Add(table);
            GetSuppliers();
            doc.Close();
        }

        public void SetDocumentDetails()
        {
            if (SiteName != null)
            {
                documentTitle = SiteName.GetTitle();
                city = SiteName.GetCity();
                int.TryParse(SiteName.GetPuDay(), out puDay);
                int.TryParse(SiteName.GetPuMonth(), out puMonth);
                int.TryParse(SiteName.GetPuYear(), out puYear);
            }
            else
            {
                Debug.WriteLine("SITENAME IS EMPTY or INCORRECT");
            }
        }

        public void CreateHeaders()
        {
            int colSpan = Const.categories.Count + 1;
            table = new PdfPTable(colSpan);
            table.LockedWidth = true;//fix the absolute width of the table
            
            float[] widths = new float[colSpan];//relative col widths in proportions - 1/3 and 2/3

            for (int i = 0; i < colSpan; i++)
            {
                widths[i] = 2f;
            }
            widths[0] = 1.5f;
            widths[1] = 2.5f;

            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 10f;//leave a gap before and after the table
            table.SpacingAfter = 30f;
            table.TotalWidth = doc.Right - doc.Left;

            Font font1 = FontFactory.GetFont("Arial", 9, Font.NORMAL);

            PdfPCell cell = new PdfPCell(new Phrase(documentTitle + " " + puMonth + "-" + puDay + " " + city, font1));
            cell.Colspan = colSpan;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date", font));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);

            for (int i = 0; i < Const.categories.Count; i++)
            {
                cell = new PdfPCell(new Phrase(Const.categories.ElementAt(i).PdfClass.ToLower(), font));
                table.AddCell(cell);
            }
        }

        public void AddRow(JOffer[] offers)
        {
            try
            {
                PdfPCell cell = new PdfPCell(new Phrase(puMonth + "-" + puDay + "/" + doDate.AddDays(dayNum).Day + "\n" + dayNum, font));
                dayNum++;
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                table.AddCell(cell);

                for (int i = 0; i < offers.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(offers[i].GetOffer(), font));

                    var chunk = new Chunk(offers[i].GetOffer(), font);

                    List<SupplierNew> distSuppliers = offers[i].GetDistinctSuppliers();
                    SupplierNew other = offers[i].Suppliers.FirstOrDefault(f => f.SupplierType == 4);
                    SupplierNew best = offers[i].Suppliers.FirstOrDefault(f => f.SupplierType == 3);
                    SupplierNew carsRent = offers[i].Suppliers.FirstOrDefault(f => f.SupplierType == 2);
                    SupplierNew gm = offers[i].Suppliers.FirstOrDefault(f => f.SupplierType == 1);

                    if (other != null)
                        AddSuppliers(other.SupplierName);//best
                    if (best != null)
                        AddSuppliers(best.SupplierName);//best
                    if (carsRent != null)
                        AddSuppliers(carsRent.SupplierName);// carsrent

                    if (i == 0)
                        chunk.SetAnchor(offers[i].GetSiteName());// if mini add site name

                    cell.AddElement(chunk);
                    if (gm != null && other != null && other.Price > gm.Price && gm.Price > 0)
                    {
                        cell.BackgroundColor = new BaseColor(77, 148, 255);
                        if (other.Price - gm.Price < 2.5f && other.Price - gm.Price > 0)
                        {
                            cell.BackgroundColor = new BaseColor(255, 255, 179);
                            if (other.Price - gm.Price < 1.5f && other.Price - gm.Price > 0)
                                cell.BackgroundColor = new BaseColor(128, 255, 128);
                        }
                    }
                    else
                        cell.BackgroundColor = new BaseColor(255, 102, 102);

                    table.AddCell(cell);
                }
                table.CompleteRow();
            }
            catch (Exception e) {
                Log.Instance.Warn("-PdfBuilder.AddRow-" + e.InnerException + e.Message);
            }
        }

        public void GetSuppliers()
        {
            doc.Add(new Paragraph(suppliers, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
        }

        public void AddSuppliers(string supplier)
        {
            if (supplier != null && !suppliers.Contains(supplier))
                suppliers += supplier + "- " + (supplier.Length > 3 ? supplier.ToLower().Substring(0, 4) : supplier.ToLower().Substring(0, 3)) + "\n";
        }

        public void Dispose()
        {
            doc.Add(table);
            GetSuppliers();
            doc.Close();
        }
    }
}
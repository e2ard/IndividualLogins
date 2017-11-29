using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndividualLogins.Controllers.App_Code
{
    public class Category
    {
        public string PdfClass;
        public string SiteClass;
        public string Name;

        public Category(string siteClass, string pdfClass, string name)
        {
            PdfClass = pdfClass;
            SiteClass = siteClass;
            Name = name;
        }
    }
    
    public static class Const
    {
        public static List<Category> categories = new List<Category>() {
            new Category("mini", "MCMR|MDMR", "MiniM"), 
            new Category("economy", "ECMR|EDMR", "EconomyM"), 
            new Category("economy", "ECAR|EDAR", "EconomyA"),
            new Category("compact", "CDMR|CCMR", "CompactM"),
            new Category("compact", "CDAR|CCAR", "CompactA"),
            new Category("intermediate", "ICMR|IDMR", "IntermediateM"),
            new Category("intermediate", "ICAR|IDAR", "IntermediateA"),
            new Category("standard", "SDMR", "StandardM"),
            new Category("standard", "SDAR", "StandardA"),
            new Category("estate", "SWMR", "EstateM"),
            new Category("estate", "SWAR", "EstateA"),
            new Category("CFMR", "CFMR", "CFMR"),
            new Category("CFAR", "CFAR", "CFAR"),
            new Category("suvs", "SUV", "SUVM"),
            new Category("suvs", "SUAV", "SUVA"),
            new Category("carriers_9", "PWMR", "People CarrierM"),
        };
        
    };
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for JOffer
/// </summary>
public class JOffer
{
    public string supplier;
    public string gmSupplier;
    public string crSupplier;
    public string bestSupplier;
    public float price;
    public float gmPrice;
    public float crPrice;
    public float bestPrice;
    public string siteName;
    public string transmission;
    public string category;
    public string seats;
    public string carName;
    public List<SupplierNew> Suppliers;

    private void SetSupplierPrice(string splr, string prc)
    {
        switch (splr.ToUpper())
        {
            case "GREEN MOTION":
            case "GREENMOTION":
            case "GREEN_MOTION":
                SetGM(splr);
                SetGMPrice(prc);
                break;
            case "CARSRENT":
            case "CARS RENT":
            case "CARS_RENT":
                SetCR(splr);
                SetCRPrice(prc);
                break;
            case "":
                SetBest();
                SetBestPrice(prc);
                break;
            default:
                SetSupplier(splr);
                SetPrice(prc);
                break;
        }
        Suppliers = new List<SupplierNew>();
    }
    public JOffer(string splr, float prc)
    {
        switch (splr.ToUpper())
        {
            case "GREEN MOTION":
            case "GREEN_MOTION":
                SetGM(splr);
                SetGMPrice(prc);
                break;
            case "CARSRENT":
            case "CARS RENT":
            case "CARS_RENT":
                SetCR(splr);
                SetCRPrice(prc);
                break;
            case "":
                SetBest();
                SetBestPrice(prc);
                break;
            default:
                SetSupplier(splr);
                SetPrice(prc);
                break;
        }
        Suppliers = new List<SupplierNew>();
    }

    public JOffer()
    {
        Suppliers = new List<SupplierNew>();
    }
    public JOffer(string splr, string prc)
    {
        SetSupplierPrice(splr, prc);
        Suppliers = new List<SupplierNew>();
    }
    public JOffer(string supplier, string price, string category, string transm, string seats)
    {
        SetSupplierPrice(supplier, price);
        this.category = category;
        transmission = transm.Trim().Split(' ')[0].Substring(0, 1);
        this.seats = seats.Trim().Substring(0, 1);
        Suppliers = new List<SupplierNew>();
    }

    public float GetMinPrice()
    {
        SupplierNew s = Suppliers.OrderBy(t => t.Price).FirstOrDefault(s1 => s1.SupplierType == 4 && s1.Price > 0);
        return s != null? s.Price: 0;
    }

    private SupplierNew GetByTypeSupplier(int type)
    {
        return Suppliers.OrderBy(p => p.Price).FirstOrDefault(s => s.SupplierType == type);
    }

    public SupplierNew GetBestSupplier()
    {
        return GetByTypeSupplier(3);
    }
    public SupplierNew GetOtherSupplier()
    {
        return GetByTypeSupplier(4);
    }

    public SupplierNew GetGmSupplier()
    {
        return GetByTypeSupplier(1);
    }

    public SupplierNew GetCrSupplier()
    {
        return GetByTypeSupplier(2);
    }
    public float GetMinGmPrice()
    {
        SupplierNew s = Suppliers.OrderBy(t => t.Price).FirstOrDefault(s1 => s1.SupplierType == 1 && s1.Price > 0);
        return s != null ? s.Price : 0;
    }
    public void AddSupplier(SupplierNew sp)
    {
        Suppliers.Add(sp);
    }
    private void SetBest()
    {
        bestSupplier = "Best";
    }


    public void SetSupplier(string splr)
    {
        supplier = splr;
    }

    public void SetCategory(string ctr)
    {
        category = MapCategory(ctr);
    }

    public string GetSiteName()
    {
        return siteName;
    }

    public void SetSiteName(string site)
    {
        siteName = site;
    }

    public void SetPrice(string price)
    {
        this.price = ParsePrice(price);
    }

    public void SetPrice(float price)
    {
        this.price = price;
    }

    public void SetGM(string splr)
    {
        this.gmSupplier = splr;
    }

    public void SetGMPrice(string price)
    {
        this.gmPrice = ParsePrice(price);
    }

    public void SetGMPrice(float price)
    {
        this.gmPrice = price;
    }

    public void SetBestPrice(string price)
    {
        this.bestPrice = ParsePrice(price);
    }

    public void SetBestPrice(float price)
    {
        this.bestPrice = price;
    }

    public float GetBestPrice()
    {
        return this.bestPrice;
    }

    public void SetCR(string splr)
    {
        this.crSupplier = splr;
    }

    public string GetCR()
    {
        return this.crSupplier;
    }

    public void SetCRPrice(string price)
    {
        this.crPrice = ParsePrice(price);
    }

    private float ParsePrice(string price)
    {
        if (!string.IsNullOrEmpty(price))
            return float.Parse(Regex.Match(price.Replace(',', '.'), "\\d+\\.?\\d+").Value, CultureInfo.InvariantCulture);
        else
            return 0;
    }

    public void SetCRPrice(float price)
    {
        this.crPrice = price;
    }

    public float GetCRPrice()
    {
        return this.crPrice;
    }

    public string toString()
    {
        return supplier + " " + price + " " + category;
    }

    protected string IsEmpty(string strToCheck)
    {
        if (strToCheck.Equals(""))
            return strToCheck;
        else
            return strToCheck + "\n";
    }

    protected string IsEmptySupplier(SupplierNew sup)
    {
        if (sup != null && sup.SupplierName != null)
        {
            if (sup.SupplierName.Equals(""))
                return "-" + " " + sup.Price + "\n";
            else
                return (sup.SupplierName.Length > 3 ? sup.SupplierName.ToLower().Substring(0, 4) : sup.SupplierName.ToLower().Substring(0, 3)) + " " + sup.Price + "\n";
        }
        return "";
    }

    public string GetOffer()
    {
        try
        {
            string output = string.Empty;

            SupplierNew other = Suppliers.OrderBy(s => s.Price).FirstOrDefault(f => f.SupplierType == 4);
            SupplierNew best = Suppliers.OrderBy(s => s.Price).FirstOrDefault(f => f.SupplierType == 3);
            SupplierNew cr = Suppliers.OrderBy(s => s.Price).FirstOrDefault(f => f.SupplierType == 2);
            SupplierNew gm = Suppliers.OrderBy(s => s.Price).FirstOrDefault(f => f.SupplierType == 1);

            List<SupplierNew> suppliersNew = new List<SupplierNew>();//Suppliers.GroupBy(g => g.SupplierType, (key, s) => s.OrderBy(e => e.Price).First()).ToList();

            if (Suppliers.Count(s => s.SupplierType == 4) > 1)
                suppliersNew.AddRange(Suppliers.Where(s => s.SupplierType == 4).OrderBy(p => p.Price).Take(2).ToList());
            else if (Suppliers.FirstOrDefault(s => s.SupplierType == 4) != null)
                suppliersNew.Add(other);

            if (gm != null)
                suppliersNew.Add(gm);
            if (cr != null)
                suppliersNew.Add(cr);
            if (best != null)
                suppliersNew.Add(best);
            //if (other != null)
            //    suppliersNew.Add(other);

            foreach (SupplierNew sup in suppliersNew.OrderBy(t => t.Price))
            {
                output += IsEmptySupplier(sup);
            }
            return output;
        }
        catch(Exception e)
        {
            return null;
        }
    }

    public List<SupplierNew> GetDistinctSuppliers()
    {
        SupplierNew gm = Suppliers.FirstOrDefault(s => s.SupplierType == 1);
        SupplierNew cr = Suppliers.FirstOrDefault(s => s.SupplierType == 2);
        SupplierNew best = Suppliers.FirstOrDefault(s => s.SupplierType == 3);
        List<SupplierNew> suppliersNew = new List<SupplierNew>();//Suppliers.GroupBy(g => g.SupplierType, (key, s) => s.OrderBy(e => e.Price).First()).ToList();

        if (Suppliers.Count(s => s.SupplierType == 4) > 1)
            suppliersNew.AddRange(Suppliers.Where(s => s.SupplierType == 4).OrderBy(p => p.Price).Take(2).ToList());
        else if (Suppliers.FirstOrDefault(s => s.SupplierType == 4) != null)
            suppliersNew.Add(Suppliers.FirstOrDefault(s => s.SupplierType == 4));

        return suppliersNew;
    }

    private string MapCategory(string category)
    {
        switch (category)
        {
            case "MCMR":
            case "MCMN":
            case "MDMR":
            case "NDMR":
                return "MiniM";
            case "EDMR":
            case "EDMN":
            case "ECMN":
            case "ECMR":
                return "EconomyM";
            case "EDAR":
            case "ECAR":
            case "ECAN":
            case "EDAN":
                return "EconomyA";
            case "CDMR":
            case "CCMR":
            case "CDMN":
            case "CCMN":
                return "CompactM";
            case "CDAR":
            case "CCAR":
            case "CDAN":
            case "CCAN":
                return "CompactA";
            case "IDMR":
            case "ICMR":
            case "IDMD":
                return "IntermediateM";
            case "IDAR":
            case "ICAR":
            case "IDAD":
                return "IntermediateA";
            case "SDMR":
            case "SCMR":
                return "StandardM";
            case "SDAR":
            case "SCAR":
                return "StandardA";
            case "SWMR":
            case "IWMR":
            case "EWMR":
            case "EWMH":
            case "CWMR":
                return "EstateM";
            case "SWAR":
            case "IWAR":
            case "EWAR":
            case "EWAH":
            case "CWAR":
                return "EstateA";
            case "CFMR":
            case "EFMR":
                return "CFMR";
            case "CFAR":
            case "EFAR":
                return "CFAR";
            case "IFMR":
            case "IFMD":
            case "IFMN":
            case "SFMR":
            case "PFMR":
                return "SUVM";
            case "IFAR":
            case "IFAD":
            case "IFAN":
            case "SFAR":
            case "PFAR":
                return "SUVA";
            case "PWMR":
            case "SVMR":
                return "People CarrierM";
            default:
                //System.Diagnostics.Debug.WriteLine(category);
                break;
        }
        return "skip";
    }

    public class Supplier
    {
        public string name;
        public float price;
    }

    public class SupplierNew
    {
        public string SupplierName;
        public float Price;
        public string Transmission;
        public float Seats;
        public string CarName;
        public string Category;
        public int SupplierType;

        public SupplierNew()
        {

        }

        public SupplierNew(string supplier, string price)
        {
            SupplierName = supplier;
            Price = ParsePrice(price);
            SupplierType = SetSupplierType(supplier);
        }

        public SupplierNew(string supplier, string price, string category, string transm, string seats, string carName = null)
        {
            SupplierName = supplier;
            Category = category;
            Transmission = ParseTransmission(transm);
            Price = ParsePrice(price);
            Seats = ParseSeats(seats.Trim().Substring(0, 1));
            SupplierType = SetSupplierType(supplier);
            CarName = carName;
        }

        private string MapCategory(string category)
        {
            switch (category)
            {
                case "MCMR":
                case "MCMN":
                case "MDMR":
                    return "MiniM";
                case "EDMR":
                case "EDMN":
                case "ECMN":
                case "ECMR":
                case "EWMR":
                case "EWMH":
                    return "EconomyM";
                case "EDAR":
                case "ECAR":
                case "ECAN":
                case "EDAN":
                case "EWAR":
                case "EWAH":
                    return "EconomyA";
                case "CDMR":
                case "CCMR":
                case "CDMN":
                case "CCMN":
                case "CWMR":
                    return "CompactM";
                case "CDAR":
                case "CCAR":
                case "CDAN":
                case "CCAN":
                case "CWAR":
                    return "CompactA";
                case "IDMR":
                case "ICMR":
                    return "IntermediateM";
                case "IDAR":
                case "ICAR":
                    return "IntermediateA";
                case "SDMR":
                case "SCMR":
                    return "StandardM";
                case "SDAR":
                case "SCAR":
                    return "StandardA";
                case "SWMR":
                case "IWMR":
                    return "EstateM";
                case "SWAR":
                case "IWAR":
                    return "EstateA";
                case "CFMR":
                case "EFMR":
                    return "CFMR";
                case "CFAR":
                case "EFAR":
                    return "CFAR";
                case "IFMR":
                case "IFMD":
                case "IFMN":
                case "SFMR":
                case "PFMR":
                    return "SUVM";
                case "IFAR":
                case "IFAD":
                case "IFAN":
                case "SFAR":
                case "PFAR":
                    return "SUVA";
                case "PWMR":
                case "SVMR":
                    return "People CarrierM";
                default:
                    //System.Diagnostics.Debug.WriteLine(category);
                    break;
            }
            return "skip" + category;
        }

        private string ParseTransmission(string transm)
        {
            return string.IsNullOrEmpty(transm)? "": transm.Trim().Split(' ')[0].Substring(0, 1);
        }
        private float ParsePrice(string price)
        {
            if (!string.IsNullOrEmpty(price))
                return float.Parse(Regex.Match(price.Replace(',', '.'), "\\d+\\.?\\d+").Value, CultureInfo.InvariantCulture);
            else
                return 0;
        }

        private float ParseSeats(string seats)
        {
            if (!string.IsNullOrEmpty(seats))
                return float.Parse(Regex.Match(seats.Replace(',', '.'), "\\d+").Value, CultureInfo.InvariantCulture);
            else
                return 0;
        }

        private int SetSupplierType(string splr)
        {
            if (splr == null || splr.Equals(""))
                return 3;

            switch (splr.ToUpper())
            {
                case "GREEN MOTION":
                case "GREENMOTION":
                case "GREEN_MOTION":
                    return 1;
                case "CARSRENT":
                case "CARS RENT":
                case "CARS_RENT":
                    return 2;
                default:
                    return 4;
            }
        }

        public void SetCategory(string ctr)
        {
            Category = MapCategory(ctr);
        }

        public string ToString()
        {
            if (SupplierName != null)
            {
                if (SupplierName.Equals(""))
                    return "-" + " " + Price + "\n";
                else
                    return (SupplierName.Length > 3 ? SupplierName.ToLower().Substring(0, 4) : SupplierName.ToLower().Substring(0, 3)) + " " + Price + "\n";
            }
            return "";
        }
    }
}


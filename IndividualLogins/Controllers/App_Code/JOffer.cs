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
    }

    public JOffer(string splr, string prc)
    {
        SetSupplierPrice(splr, prc);
    }
    public JOffer (string supplier, string price, string category, string transm, string seats)
    {
        SetSupplierPrice(supplier, price);
        this.category = category;
        transmission = transm.Trim().Split(' ')[0].Substring(0, 1);
        this.seats = seats.Trim().Substring(0, 1);
    }

    private void SetBest()
    {
        bestSupplier = "Best";
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
    }

    public JOffer() { }

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

    public string GetSupplier()
    {
        return this.supplier;
    }

    public void SetPrice(string price)
    {
        this.price = ParsePrice(price);
    }

    public void SetPrice(float price)
    {
        this.price = price;
    }

    public float GetPrice()
    {
        return this.price;
    }

    public void SetGM(string splr)
    {
        this.gmSupplier = splr;
    }

    public string GetGM()
    {
        return this.gmSupplier;
    }

    public void SetGMPrice(string price)
    {
        this.gmPrice = ParsePrice(price);
    }

    public void SetGMPrice(float price)
    {
        this.gmPrice = price;
    }

    public float GetGMPrice()
    {
        return this.gmPrice;
    }

    public string GetBest()
    {
        return this.bestSupplier;
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

    protected string IsEmptySupplier(Supplier sup)
    {
        if (sup.name != null)
        {
            if (sup.name.Equals(""))
                return "-" + " " + sup.price + "\n";
            else
                return (sup.name.Length > 3 ? sup.name.ToLower().Substring(0, 4) : sup.name.ToLower().Substring(0, 3)) + " " + sup.price + "\n";
        }
        return "";
    }

    public string GetOffer()
    {// a price b gnPrice c crPrice
        string offer = validate(supplier, price);//0
        string gmOffer = validate(gmSupplier, gmPrice);//1
        string crOffer = validate(crSupplier, crPrice);//2

        List<Supplier> suppliers = new List<Supplier>() { new Supplier { name = supplier, price = price }, new Supplier { name = gmSupplier, price = gmPrice }, new Supplier { name = crSupplier, price = crPrice }, new Supplier { name = bestSupplier, price = bestPrice } };
        string output = string.Empty;
        foreach (Supplier sup in suppliers.OrderBy(t => t.price))
        {
            output += IsEmptySupplier(sup);
        }
        return output;
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
        return "skip";
    }

    private string validate(string supplier, float price)
    {
        if (price > 10000 || price == 0)
            return "";
        if (supplier != null && supplier.Length < 3 && price > 0)
            supplier = "best";
        return supplier != null && supplier.Length > 3 ? char.ToUpper(supplier[0]).ToString() + char.ToLower(supplier[1]).ToString() + char.ToLower(supplier[2]).ToString() + " " + price : supplier + " " + price;
    }

    public class Supplier
    {
        public string name;
        public float price;
    }
}



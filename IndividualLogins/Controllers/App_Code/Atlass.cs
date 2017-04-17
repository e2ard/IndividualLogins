using IndividualLogins.Controllers.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Atlass : SiteBase
{
    DateTime origin = new DateTime(1970, 1, 1, 12, 0, 0);
    public Atlass(string url)
    {
        SetSiteName(url);
    }

    public override void InitDate(DateTime date)
    {
        SetStartDate(date);
        SetEndDate(date);
    }

    public override void SetDoDay(DateTime day)
    {
        throw new NotImplementedException();
    }

    public override string GetPuDay()
    {
        TimeSpan time = TimeSpan.FromSeconds(int.Parse(GetStartDate()) - 86400);
        DateTime dateTime = origin.Add(time);
        return dateTime.Day.ToString();
    }

    public override string GetPuMonth()
    {
        TimeSpan time = TimeSpan.FromSeconds(int.Parse(GetStartDate()));
        DateTime dateTime = origin.Add(time);
        return dateTime.Month.ToString();
    }

    public override string GetPuYear()
    {
        TimeSpan time = TimeSpan.FromSeconds(int.Parse(GetStartDate()));
        DateTime dateTime = origin.Add(time);
        return dateTime.Year.ToString();
    }

    public void SetStartDate(DateTime date)
    {
        string dateToSearch = "start=(\\d)*";
        string dateToChange = "start=" + (date - origin).TotalSeconds;

        Regex rgx = new Regex(dateToSearch);
        siteName = rgx.Replace(siteName, dateToChange);
    }

    public void SetEndDate(DateTime date)
    {
        string dateToSearch = "end=(\\d)*";
        string dateToChange = "end=" + (date - origin).TotalSeconds;

        Regex rgx = new Regex(dateToSearch);
        siteName = rgx.Replace(siteName, dateToChange);
    }

    public string GetStartDate()
    {
        return GetDocumentDetails("start=[^&]*");
    }

    public string GetEndDate()
    {
        return GetDocumentDetails("end=[^&]*");
    }

    public List<string> GetGeneratedLinksByDate(DateTime sDate, DateTime eDate)
    {
        List<string> generatedList = new List<string>();
        InitDate(sDate);
        for (int i = 1; i <= (eDate - sDate).Days; i++)
        {
            SetEndDate(sDate.AddDays(i));
            generatedList.Add(GetSiteName());
        }
        return generatedList;
    }

    public override string GetCity()
    {
        return GetLocation(GetDocumentDetails("location=[^&]*"));
    }

    private string GetDocumentDetails(string pattern)
    {
        Match matchDetails = Regex.Match(GetSiteName(), pattern);

        return matchDetails.Captures[0].Value.Substring(pattern.Length - 5, matchDetails.Captures[0].Value.Length - (pattern.Length - 5));
    }

    public override string GetTitle()
    {
        return "atlas";
    }

    public static string GetSupplier(string supplier)
    {
        switch (supplier)
        {
            case "70718":
            case "70719":
            case "70715":
            case "70761":
                return "Green Motion";
            case "48911":
            case "49019":
                return "CARS RENT";
            case "54664":
            case "42409":
            case "54665":
            case "69165":
                return "Budget";
            case "61826":
            case "56266":
                return "Avis";
            case "24380":
            case "23823":
            case "24381":
            case "24388":
                return "Europcar";
            case "71010":
            case "71012":
                return "Enterprise";
            default:
                return supplier;
        }
    }

    private string GetLocation(string location)
    {
        switch(location)
        {
            case "2328":
                return "Riga";
            case "2342":
                return  "Vilnius";
            default:
                return location;
        }
    }
}

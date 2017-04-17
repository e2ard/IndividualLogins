using IndividualLogins.Controllers.App_Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Scanner : SiteBase
{
    string pattern = "PickUpDateTime%22%3A%22201\\d-\\d\\d-[^-T]*";

    public Scanner(string site1)
    {
        SetSiteName(site1);
    }

    public override void InitDate(DateTime date)
    {
        string dateToSearch = "ReturnDateTime%22%3A%22201\\d-\\d+-\\d+";
        string dateToChange = "ReturnDateTime%22%3A%22" + date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day);

        Regex rgx = new Regex(dateToSearch);
        siteName = rgx.Replace(siteName, dateToChange);

        dateToSearch = "PickUpDateTime%22%3A%22201(\\d)-(\\d)*-(\\d)*";
        dateToChange = "PickUpDateTime%22%3A%22" + date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day);

        rgx = new Regex(dateToSearch);
        SetSiteName(rgx.Replace(siteName, dateToChange));
    }



    public override void SetDoDay(DateTime date)
    {
        string dateToChange = "ReturnDateTime%22%3A%22201\\d-\\d+-\\d+";
        string replacement = "ReturnDateTime%22%3A%22" + date.Year + "-" + AddZero(date.Month) + "-" + AddZero(date.Day);

        Regex rgx = new Regex(dateToChange);
        siteName = rgx.Replace(siteName, replacement);
    }

    public override string GetCity()
    {
        try
        {
            string pattern = "returnName%3D[^%]*";
            Match matchDetails = Regex.Match(GetSiteName(), pattern);

            string temp = matchDetails.Captures[0].Value;
            return temp.Substring(13, temp.Length - 13);

        }
        catch
        {
            return "Unknown"; //TOFIX 
        }
    }

    public override string GetPuDay()
    {
        Match matchDetails = Regex.Match(GetSiteName(), pattern);
        if (!matchDetails.Success)
        {
           matchDetails = Regex.Match(GetSiteName(), "PickUpDateTime%22:%22201(\\d)-\\d+-\\d+T\\d+:\\d+");
        }
        string temp = matchDetails.Captures[0].Value;
        return temp.Substring(temp.Length - 2, 2);
    }

    public override string GetPuMonth()
    {
        Match matchDetails = Regex.Match(GetSiteName(), pattern);
        if (!matchDetails.Success)
        {
            matchDetails = Regex.Match(GetSiteName(), "PickUpDateTime%22:%22201(\\d)-\\d+-\\d+T\\d+:\\d+");
        }
        string temp = matchDetails.Captures[0].Value;
        return temp.Substring(temp.Length - 5, 2);
    }

    public override string GetTitle()
    {
        //try
        //{
        //    string pattern = ".*(\\.).{3}\\/";
        //    Match matchDetails = Regex.Match(GetSiteName(), pattern);

        //    string title = matchDetails.Captures[0].Value.Substring(pattern.Length - 5, matchDetails.Captures[0].Value.Length - (pattern.Length - 6));

        //    return title.Substring(8, title.Length - 13);
        //}
        //catch
        {
            return "scanner";
        }
    }

    public override string GetPuYear()
    {
        return "2017";
    }
}
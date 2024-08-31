using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ZDELDoorHelper.Services;

public class ZDELDoorHelperTools : IZDELDoorHelperTools
{
    public string GetZDELDoorParameter(string uri)
    {
        string Result = "";
        string pattern = "f=(.*)$";
        //uri should be like
        //"http://web.fondcard.net/weixin/opendoor.php?f=DgHSZAEdimmZHY5pmZG"
        foreach (Match match in Regex.Matches(uri, pattern))
        {
            Result = Regex.Escape(match.Value).Substring(2, Regex.Escape(match.Value).Length - 2);

        }
        return Result;
    }
    public HttpClient CreateClient()
    {
#if ANDROID
        return new HttpClient(new Xamarin.Android.Net.AndroidMessageHandler());
#else
        return new HttpClient();
#endif
    }
    public async Task<string> SendOpenDoorHttpGetRequestAync(string uri, string mobileNumber)
    {
        if (mobileNumber == null)
        {
            return "Invalid Cookie";
        }
        using var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Add("Cookie", $"mobile_enterprise={mobileNumber}");
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return ex.Message;
        }
    }
}


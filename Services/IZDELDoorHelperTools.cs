namespace ZDELDoorHelper.Services;

public interface IZDELDoorHelperTools
{
    string GetZDELDoorParameter(string uri);
    Task<string> SendOpenDoorHttpGetRequestAync(string uri, string mobileNumber);
}

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IWebRequestFactory
    {
        IWebRequest Create(string uri);
    }
}
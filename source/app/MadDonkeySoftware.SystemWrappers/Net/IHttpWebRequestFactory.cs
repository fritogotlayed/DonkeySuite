namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IHttpWebRequestFactory
    {
        IHttpWebRequest Create(string uri);
    }
}
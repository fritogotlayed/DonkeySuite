namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IHttpWebRequest
    {
        string Method { get; set; }

        IHttpWebResponse GetResponse();
    }
}
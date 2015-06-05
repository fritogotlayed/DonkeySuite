using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IHttpWebResponse : IWebResponse
    {
        string StatusDescription { get; }
        HttpStatusCode StatusCode { get; }
    }
}
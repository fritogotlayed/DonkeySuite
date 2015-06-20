using System.IO;
using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IWebRequest
    {
        string Method { get; set; }

        string ContentType { get; set; }

        long ContentLength { get; set; }

        WebHeaderCollection Headers { get; }

        Stream GetRequestStream();

        IWebResponse GetResponse();
    }
}
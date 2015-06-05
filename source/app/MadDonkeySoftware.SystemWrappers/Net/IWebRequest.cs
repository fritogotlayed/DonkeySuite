using System.IO;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IWebRequest
    {
        string Method { get; set; }

        string ContentType { get; set; }

        long ContentLength { get; set; }

        Stream GetRequestStream();

        IWebResponse GetResponse();
    }
}
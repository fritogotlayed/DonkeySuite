using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public class HttpWebRequestFactory : IHttpWebRequestFactory
    {
        public IHttpWebRequest Create(string uri)
        {
            return new HttpWebRequestWrapper((HttpWebRequest)WebRequest.Create(uri));
        }
    }
}
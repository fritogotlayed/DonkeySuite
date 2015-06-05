using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public class WebRequestFactory : IWebRequestFactory
    {
        public IWebRequest Create(string uri)
        {
            return new WebRequestWrapper((HttpWebRequest)WebRequest.Create(uri));
        }
    }
}
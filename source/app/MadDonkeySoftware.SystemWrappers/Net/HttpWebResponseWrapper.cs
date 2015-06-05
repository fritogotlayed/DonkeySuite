using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public class HttpWebResponseWrapper : WebResponseWrapper, IHttpWebResponse
    {
        public HttpWebResponseWrapper(WebResponse response) : base(response)
        {
        }

        public string StatusDescription
        {
            get { return ((HttpWebResponse)Response).StatusDescription; }
        }

        public HttpStatusCode StatusCode
        {
            get { return ((HttpWebResponse)Response).StatusCode; }
        }
    }
}
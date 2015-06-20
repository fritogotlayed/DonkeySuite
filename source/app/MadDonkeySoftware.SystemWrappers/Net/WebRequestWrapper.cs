using System.IO;
using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public class WebRequestWrapper : IWebRequest
    {
        private readonly WebRequest _request;

        public WebRequestWrapper(WebRequest request)
        {
            _request = request;
        }

        public string Method
        {
            get { return _request.Method; }
            set { _request.Method = value; }
        }

        public string ContentType
        {
            get { return _request.ContentType; }
            set { _request.ContentType = value; }
        }

        public long ContentLength
        {
            get { return _request.ContentLength; }
            set { _request.ContentLength = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _request.Headers; }
        }

        public Stream GetRequestStream()
        {
            return _request.GetRequestStream();
        }

        public IWebResponse GetResponse()
        {
            return new HttpWebResponseWrapper(_request.GetResponse());
        }

    }
}
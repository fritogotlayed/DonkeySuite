using System;
using System.IO;
using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public class HttpWebResponseWrapper : IHttpWebResponse
    {
        private WebResponse _response;

        public HttpWebResponseWrapper(HttpWebResponse response)
        {
            _response = response;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_response != null)
                {
                    ((IDisposable)_response).Dispose();
                    _response = null;
                }
            }
        }

        public Stream GetResponseStream()
        {
            return _response.GetResponseStream();
        }
    }
}
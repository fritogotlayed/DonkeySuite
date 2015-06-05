using System;
using System.IO;
using System.Net;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public class WebResponseWrapper : IWebResponse
    {
        protected WebResponse Response { get; private set; }

        public WebResponseWrapper(WebResponse response)
        {
            Response = response;
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
                if (Response != null)
                {
                    ((IDisposable)Response).Dispose();
                    Response = null;
                }
            }
        }

        public Stream GetResponseStream()
        {
            return Response.GetResponseStream();
        }
    }
}
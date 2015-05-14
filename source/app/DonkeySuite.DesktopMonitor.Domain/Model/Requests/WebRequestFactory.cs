using System.Collections.Generic;
using System.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public static class WebRequestFactory
    {
        private static readonly Queue<WebRequest> MockedWebRequests = new Queue<WebRequest>();

        public static WebRequest CreateWebRequest(string requestUri)
        {
            return MockedWebRequests.Count > 0 ? MockedWebRequests.Dequeue() : WebRequest.Create(requestUri);
        }

        public static void AddWebRequestMock(WebRequest webRequest)
        {
            MockedWebRequests.Enqueue(webRequest);
        }
    }
}
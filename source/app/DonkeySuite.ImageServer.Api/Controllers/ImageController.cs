using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DonkeySuite.ImageServer.Api.Models;

namespace DonkeySuite.ImageServer.Api.Controllers
{
    public class ImageController : ApiController
    {
        // GET image
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET image/5
        public string Get(int id)
        {
            return "value";
        }

        // POST image
        // public void Post([FromBody] string value)
        public HttpResponseMessage Post(AddImageRequest request)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Blah message.")
            };
        }

        // PUT image/5
        public void Put(int id, [FromBody] string value)
        {
            var x = 0;
            x++;
        }

        // DELETE image/5
        public void Delete(int id)
        {
            var x = 0;
            x++;
        }

        // PATCH image/5
        [AcceptVerbs("PATCH")]
        public string Patch(int id, [FromBody] string value)
        {
            return "PATCH";
        }
    }
}

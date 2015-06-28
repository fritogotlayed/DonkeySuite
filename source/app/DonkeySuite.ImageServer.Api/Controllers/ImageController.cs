﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DonkeySuite.ImageServer.Api.Filters;
using DonkeySuite.ImageServer.Api.Models;
using log4net;
using Newtonsoft.Json;

namespace DonkeySuite.ImageServer.Api.Controllers
{
    [TokenAuthenticationFilter]
    public class ImageController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private class Foo
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }

        // GET image
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET image/5
        public HttpResponseMessage Get(int id)
        {
            var responseObject = new Foo {Value = id, Name = "Blah"};
            return new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseObject))
            };
        }

        // POST image
        // public void Post([FromBody] string value)
        public HttpResponseMessage Post(AddImageRequest request)
        {
            Log.DebugFormat("Accepted request for file {0}.", request.FileName);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Format("{0} received.", request.FileName))
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

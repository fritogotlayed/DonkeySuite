/*
<copyright file="TokenAuthenticationFilter.cs">
   Copyright 2015 MadDonkey Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
</copyright>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DonkeySuite.ImageServer.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TokenAuthenticationFilter : AuthorizationFilterAttribute
    {
        private readonly bool _active;

        public TokenAuthenticationFilter() : this(true)
        {
        }

        public TokenAuthenticationFilter(bool active)
        {
            _active = active;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!_active) return;

            IEnumerable<string> authorizationTokens;
            actionContext.Request.Headers.TryGetValues("X-Auth-Token", out authorizationTokens);

            if (authorizationTokens == null || !authorizationTokens.Any()) Challenge(actionContext);

            base.OnAuthorization(actionContext);
        }

        protected static void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }
}
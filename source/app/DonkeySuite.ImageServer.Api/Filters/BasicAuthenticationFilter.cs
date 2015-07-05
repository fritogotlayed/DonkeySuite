/*
<copyright file="BasicAuthenticationFilter.cs">
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
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DonkeySuite.ImageServer.Api.Domain;

namespace DonkeySuite.ImageServer.Api.Filters
{
    /// <summary>
    /// Generic Basic Authentication filter that checks for basic authentication
    /// headers and challenges for authentication if no authentication is provided
    /// Sets the Thread Principle with a GenericAuthenticationPrincipal.
    /// 
    /// You can override the OnAuthorize method for custom authorization logic that
    /// might be application specific.
    /// </summary>
    /// <remarks>Always remember that Basic Authentication passes username and passwords
    /// from client to server in plain text, so make sure SSL is used with basic authorization
    /// to encode the Authorization header on all requests (not just the login).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BasicAuthenticationFilter : AuthorizationFilterAttribute
    {
        private readonly bool _active;

        /// <summary>
        /// Constructor creates enabled basic authorization filter.
        /// </summary>
        public BasicAuthenticationFilter() : this(true)
        {
        }

        /// <summary>
        /// Overridden constructor to allow explicit disabling of this
        /// filter's behavior. Pass false to disable (same as no filter
        /// but declarative)
        /// </summary>
        /// <param name="active">
        /// If set to <c>true</c> the authorization filter is active.
        /// </param>
        public BasicAuthenticationFilter(bool active)
        {
            _active = active;
        }


        /// <summary>
        /// Override to Web API filter method to handle Basic Authorization check
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!_active) return;

            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == null)
            {
                Challenge(actionContext);
                return;
            }


            if (!OnAuthorizeUser(identity.Name, identity.Password, actionContext))
            {
                Challenge(actionContext);
                return;
            }

            var principal = new GenericPrincipal(identity, null);

            Thread.CurrentPrincipal = principal;

            // inside of ASP.NET this is required
            //if (HttpContext.Current != null)
            //    HttpContext.Current.User = principal;

            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// Base implementation for user authentication - you probably will
        /// want to override this method for application specific logic.
        /// 
        /// The base implementation merely checks for username and password
        /// present and set the Thread principal.
        /// 
        /// Override this method if you want to customize Authentication
        /// and store user data as needed in a Thread Principle or other
        /// Request specific storage.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }

        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authorizationHeader = null;
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization != null && authorization.Scheme == "Basic")
                authorizationHeader = authorization.Parameter;

            if (string.IsNullOrEmpty(authorizationHeader))
                return null;

            authorizationHeader = Encoding.Default.GetString(Convert.FromBase64String(authorizationHeader));

            var tokens = authorizationHeader.Split(':');
            return tokens.Length < 2 ? null : new BasicAuthenticationIdentity(tokens[0], tokens[1]);
        }


        /// <summary>
        /// Send the Authentication Challenge request
        /// </summary>
        /// <param name="actionContext"></param>
        private static void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }
}
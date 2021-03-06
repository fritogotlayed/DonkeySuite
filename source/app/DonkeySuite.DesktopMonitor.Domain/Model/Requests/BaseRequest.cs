﻿/*
<copyright file="BaseRequest.cs">
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
using System.IO;
using System.Net;
using System.Text;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using log4net;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public abstract class BaseRequest : IBaseRequest
    {
        private readonly IWebRequestFactory _webRequestFactory;
        private readonly ICredentialRepository _credentialRepository;
        private static ILog _log;

        public virtual string RequestUrl { get; set; }
        public virtual string ResponseStatus { get; protected set; }
        public virtual string ResponseData { get; protected set; }

        protected BaseRequest(IWebRequestFactory webRequestFactory, ILogProvider logProvider, ICredentialRepository credentialRepository)
        {
            _webRequestFactory = webRequestFactory;
            _log = logProvider.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            _credentialRepository = credentialRepository;
        }

        public virtual bool Post()
        {
            try
            {
                // Create a request using a URL that can receive a post. 
                var request = _webRequestFactory.Create(RequestUrl);
                request.Method = "POST";
                request.ContentType = @"application/x-www-form-urlencoded";
                request.Headers["X-Auth-Token"] = _credentialRepository.GetApiKey();

                var parameters = new Dictionary<string, string>();
                PopulateRequestParameters(parameters);

                // Pack the parameters for form encoding.
                var buffer = new StringBuilder();
                var prefix = string.Empty;
                foreach (var parameter in parameters)
                {
                    buffer.AppendFormat("{0}{1}={2}", prefix, parameter.Key, parameter.Value);
                    prefix = "&";
                }

                // Encode the body for the request
                var byteArray = Encoding.UTF8.GetBytes(buffer.ToString());
                request.ContentLength = byteArray.Length;

                // Get the request stream and write the data to the request stream.
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response and update the status
                _log.DebugFormat("Performing post to \"{0}\".", RequestUrl);
                var response = (IHttpWebResponse) request.GetResponse();
                ResponseStatus = response.StatusDescription;
                _log.DebugFormat("Received response with code [{0}].", response.StatusCode);

                // Pull the response data out and place it into the corresponding property.
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        _log.DebugFormat("Reading response data for last request.");
                        var reader = new StreamReader(responseStream);
                        ResponseData = reader.ReadToEnd();
                    }
                    else
                    {
                        _log.DebugFormat("Response data is null for last request. Bypassing parsing.");
                    }
                }

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException ex)
            {
                _log.Error(string.Format("Failed posting to server.{0}{1}", Environment.NewLine, this), ex);
                return false;
            }
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.AppendLine(string.Format("Type : {0}", GetType()));
            buffer.AppendLine(string.Format("RequestUrl : {0}", RequestUrl));
            return buffer.ToString();
        }

        protected abstract void PopulateRequestParameters(Dictionary<string, string> parameters);
    }
}
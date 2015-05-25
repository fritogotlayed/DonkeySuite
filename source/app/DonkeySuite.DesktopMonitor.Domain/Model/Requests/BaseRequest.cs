﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using log4net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public abstract class BaseRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public virtual string RequestUrl { get; set; }
        public virtual string ResponseStatus { get; protected set; }
        public virtual string ResponseData { get; protected set; }

        public virtual bool Post()
        {
            try
            {
                // Create a request using a URL that can receive a post. 
                var request = WebRequestFactory.CreateWebRequest(RequestUrl);
                request.Method = "POST";
                request.ContentType = @"application/x-www-form-urlencoded";

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
                Log.InfoFormat("Performing post to \"{0}\".", RequestUrl);
                var response = (HttpWebResponse) request.GetResponse();
                ResponseStatus = response.StatusDescription;
                Log.InfoFormat("Received response with code [{0}].", response.StatusCode);

                // Pull the response data out and place it into the corresponding property.
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        Log.InfoFormat("Reading response data for last request.");
                        var reader = new StreamReader(responseStream);
                        ResponseData = reader.ReadToEnd();
                    }
                    else
                    {
                        Log.InfoFormat("Response data is null for last request. Bypassing parsing.");
                    }
                }

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException ex)
            {
                Log.Error("Failed posting to server.", ex);
                return false;
            }
        }

        protected abstract void PopulateRequestParameters(Dictionary<string, string> parameters);
    }
}
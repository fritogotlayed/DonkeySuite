/*
<copyright file="AddImageRequest.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public class AddImageRequest : BaseRequest, IAddImageRequest
    {
        private string _url;

        public AddImageRequest(IWebRequestFactory webRequestFactory, ILogProvider logProvider, ICredentialRepository credentialRepository) : base(webRequestFactory, logProvider, credentialRepository)
        {
        }

        public override string RequestUrl
        {
            get { return base.RequestUrl; }
            set
            {
                if (value.EndsWith("/"))
                {
                    _url = value + "image";
                }
                else
                {
                    _url = value + "/image";
                }

                base.RequestUrl = _url;
            }
        }

        public virtual string FileName { get; set; }

        public virtual byte[] FileBytes { get; set; }

        protected override void PopulateRequestParameters(Dictionary<string, string> parameters)
        {
            parameters.Add("fileName", FileName);
            parameters.Add("payload", Convert.ToBase64String(FileBytes));
        }
    }
}
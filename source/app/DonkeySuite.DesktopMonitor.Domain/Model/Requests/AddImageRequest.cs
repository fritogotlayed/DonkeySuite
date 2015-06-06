using System;
using System.Collections.Generic;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public class AddImageRequest : BaseRequest, IAddImageRequest
    {
        private string _url;

        public AddImageRequest(IWebRequestFactory webRequestFactory, ILogProvider logProvider) : base(webRequestFactory, logProvider)
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
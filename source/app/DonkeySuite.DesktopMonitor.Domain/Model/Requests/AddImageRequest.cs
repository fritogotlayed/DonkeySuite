using System;
using System.Collections.Generic;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public class AddImageRequest : BaseRequest
    {
        private string _url;

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

        public string FileName { get; set; }

        public byte[] FileBytes { get; set; }


        protected override void PopulateRequestParameters(Dictionary<string, string> parameters)
        {
            parameters.Add("fileName", FileName);
            parameters.Add("payload", Convert.ToBase64String(FileBytes));
        }
    }
}
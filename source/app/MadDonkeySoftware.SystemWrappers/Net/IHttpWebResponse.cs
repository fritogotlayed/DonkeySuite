using System;
using System.IO;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IHttpWebResponse : IDisposable
    {
        Stream GetResponseStream();
    }
}
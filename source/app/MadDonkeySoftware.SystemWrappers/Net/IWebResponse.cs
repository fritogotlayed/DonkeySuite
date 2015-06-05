using System;
using System.IO;

namespace MadDonkeySoftware.SystemWrappers.Net
{
    public interface IWebResponse : IDisposable
    {
        Stream GetResponseStream();
    }
}
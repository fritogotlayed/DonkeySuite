using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public interface IRequestProvider
    {
        IAddImageRequest ProvideNewAddImageRequest(IImageServer server, string fileName, byte[] imageBytes);
    }
}
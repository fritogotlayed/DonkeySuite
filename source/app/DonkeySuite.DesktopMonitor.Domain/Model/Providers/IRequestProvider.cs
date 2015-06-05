using DonkeySuite.DesktopMonitor.Domain.Model.Requests;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public interface IRequestProvider
    {
        IAddImageRequest ProvideNewAddImageRequest(string fileName, byte[] imageBytes);
    }
}
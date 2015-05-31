using DonkeySuite.DesktopMonitor.Domain.Model.Requests;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public interface IRequestProvider
    {
        AddImageRequest ProvideNewAddImageRequest(string fileName, byte[] imageBytes);
    }
}
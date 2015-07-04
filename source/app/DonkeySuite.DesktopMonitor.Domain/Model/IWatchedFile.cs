using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public interface IWatchedFile
    {
        string FullPath { get; set; }
        string FileName { get; set; }
        bool UploadSuccessful { get; set; }
        ISortStrategy SortStrategy { get; set; }
        byte[] LoadImageBytes();
        void SendToServer(IImageServer server);
        bool IsInBaseDirectory(string directory);
        void SortFile();
        void RemoveFromDisk();
    }
}
namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public interface IAddImageRequest
    {
        string RequestUrl { get; set; }
        string FileName { get; set; }
        byte[] FileBytes { get; set; }
        string ResponseStatus { get; }
        string ResponseData { get; }
        bool Post();
        string ToString();
    }
}
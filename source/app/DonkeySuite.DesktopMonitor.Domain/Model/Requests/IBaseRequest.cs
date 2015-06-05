namespace DonkeySuite.DesktopMonitor.Domain.Model.Requests
{
    public interface IBaseRequest
    {
        string RequestUrl { get; set; }
        string ResponseStatus { get; }
        string ResponseData { get; }
        bool Post();
        string ToString();
    }
}
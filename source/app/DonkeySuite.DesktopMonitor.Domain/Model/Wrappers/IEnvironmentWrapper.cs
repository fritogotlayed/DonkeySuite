namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public interface IEnvironmentWrapper
    {
        bool IsWindowsPlatform { get; }
        string UserHomeDirectory { get; }
    }
}
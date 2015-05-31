namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public interface IEnvironmentUtility
    {
        char DirectorySeparatorChar { get; }
        string UserHomeDirectory { get; }
        bool IsWindowsPlatform { get; }
    }
}
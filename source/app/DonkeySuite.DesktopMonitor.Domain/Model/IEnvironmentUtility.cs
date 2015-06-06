namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public interface IEnvironmentUtility
    {
        char DirectorySeparatorChar { get; }
        string UserHomeDirectory { get; }
        bool IsWindowsPlatform { get; }
        string CombinePath(string path1, string path2);
    }
}
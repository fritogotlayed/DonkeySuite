namespace DonkeySuite.SystemWrappers.Interfaces
{
    public interface IEnvironment
    {
        bool IsWindowsPlatform { get; }
        string UserHomeDirectory { get; }
        char DirectorySeparatorChar { get; }
    }
}
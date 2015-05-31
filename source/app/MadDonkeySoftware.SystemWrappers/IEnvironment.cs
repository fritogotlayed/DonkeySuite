namespace MadDonkeySoftware.SystemWrappers
{
    public interface IEnvironment
    {
        bool IsWindowsPlatform { get; }
        string UserHomeDirectory { get; }
    }
}
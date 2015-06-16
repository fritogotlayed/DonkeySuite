namespace MadDonkeySoftware.SystemWrappers.IO
{
    public interface IPath
    {
        char DirectorySeparatorChar { get; }
        string Combine(string path1, string path2);
        string GetFileName(string path);
        string GetExtension(string path);
    }
}
using System.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public interface ISerializer
    {
        void Serialize(TextWriter writer, object o);

        object Deserialize(Stream stream);
    }
}
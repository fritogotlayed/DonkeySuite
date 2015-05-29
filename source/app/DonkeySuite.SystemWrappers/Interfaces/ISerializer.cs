using System.IO;

namespace DonkeySuite.SystemWrappers.Interfaces
{
    public interface ISerializer
    {
        void Serialize(TextWriter writer, object o);

        object Deserialize(Stream stream);
    }
}
using System.IO;

namespace MadDonkeySoftware.SystemWrappers.Xml.Serialization
{
    public interface IXmlSerializer
    {
        void Serialize(TextWriter writer, object o);

        object Deserialize(Stream stream);
    }
}
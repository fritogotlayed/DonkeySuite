using System.IO;
using System.Xml.Serialization;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public class XmlSerializerWrapper : ISerializer
    {
        private readonly XmlSerializer _serializer;

        public XmlSerializerWrapper(XmlSerializer serializer)
        {
            this._serializer = serializer;
        }

        public void Serialize(TextWriter writer, object o)
        {
            _serializer.Serialize(writer, o);
        }

        public object Deserialize(Stream stream)
        {
            return _serializer.Deserialize(stream);
        }
    }
}
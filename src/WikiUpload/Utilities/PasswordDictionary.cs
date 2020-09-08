using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WikiUpload
{

    [XmlRoot("Root")]
    public class PasswordDictionary
        : Dictionary<string, string>, IXmlSerializable
    {
        private const string entryElementName = "Entry";
        private const string keyAttributeName = "Key";
        private const string valueAttributeName = "Value";

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.Name == entryElementName)
                    {
                        var key = reader.GetAttribute(keyAttributeName);
                        var value = reader.GetAttribute(valueAttributeName);
                        if (key != null && value != null)
                            Add(key, value);
                    }
                    reader.Read();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in this.Keys)
            {
                writer.WriteStartElement(entryElementName);
                writer.WriteAttributeString(keyAttributeName, key);
                writer.WriteAttributeString(valueAttributeName, this[key]);
                writer.WriteEndElement();
            }
        }
    }
}

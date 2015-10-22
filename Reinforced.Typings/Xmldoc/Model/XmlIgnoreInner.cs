using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Reinforced.Typings.Xmldoc.Model
{
    public abstract class XmlIgnoreInner : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public abstract void ReadXml(XmlReader reader);

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Xml;
using System.Xml.Serialization;
#pragma warning disable 1591

namespace Reinforced.Typings.Xmldoc.Model
{
    [XmlRoot("doc")]
    [XmlType("doc")]
    [XmlInclude(typeof(DocumentationMember))]
    [XmlInclude(typeof(DocumentationParameter))]
    public class Documentation
    {
        [XmlArray("members")]
        [XmlArrayItem("member")]
        public DocumentationMember[] Members { get; set; }
    }

    public class DocumentationMember
    {
        private string _name;

        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                MemberType = _name.MeberType();
            }
        }

        [XmlIgnore]
        public DocumentationMemberType MemberType { get; private set; }

        [XmlElement(ElementName = "summary")]
        public DocumentationSummary Summary { get; set; }

        [XmlElement(ElementName = "param")]
        public DocumentationParameter[] Parameters { get; set; }

        public override string ToString()
        {
            return String.Format("({0}) {1}", MemberType, Name);
        }

        [XmlElement(ElementName = "returns")]
        public DocumentationReturns Returns { get; set; }
    }


    public class DocumentationParameter : XmlIgnoreInner
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute("name");
            Description = reader.ReadInnerXml().Trim();
            if (Description.Contains("this method interface declaration"))
            {

            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DocumentationSummary : XmlIgnoreInner
    {
        public string Text { get; set; }
        public string Cref { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public override void ReadXml(XmlReader reader)
        {
            Cref = reader.GetAttribute("cref");
            Text = reader.ReadInnerXml().Trim();
        }
    }

    public class DocumentationReturns : XmlIgnoreInner
    {
        [XmlText]
        public string Text { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            Text = reader.ReadInnerXml().Trim();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}

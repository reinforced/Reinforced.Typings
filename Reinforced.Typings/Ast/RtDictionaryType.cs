using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public class RtDictionaryType : RtTypeName
    {
        public RtDictionaryType()
        {

        }

        public RtDictionaryType(string keySimpleType,string valueSimpleType)
        {
            KeyType = new RtSimpleTypeName(keySimpleType);
            ValueType = new RtSimpleTypeName(valueSimpleType);
        }

        public RtDictionaryType(RtTypeName keySimpleType, RtTypeName valueSimpleType)
        {
            KeyType = keySimpleType;
            ValueType = valueSimpleType;
        }

        public RtTypeName KeyType { get; private set; }

        public RtTypeName ValueType { get; private set; }

        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return KeyType;
                yield return ValueType;
            }
        }

        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

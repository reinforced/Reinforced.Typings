using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public class RtArrayType : RtTypeName
    {
        public RtTypeName ElementType { get; private set; }

        public RtArrayType(RtTypeName elementType)
        {
            ElementType = elementType;
        }

        public override IEnumerable<RtNode> Children
        {
            get { yield return ElementType; }
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

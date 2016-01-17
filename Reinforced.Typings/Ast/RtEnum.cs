using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public class RtEnum : RtCompilationUnit
    {
        public RtJsdocNode Documentation { get; set; }

        public RtSimpleTypeName EnumName { get; set; }

        public List<RtEnumValue> Values { get; private set; }

        public RtEnum()
        {
            Values = new List<RtEnumValue>();
        }

        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
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

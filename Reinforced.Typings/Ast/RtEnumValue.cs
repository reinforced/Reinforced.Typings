using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public class RtEnumValue : RtNode
    {
        public RtJsdocNode Documentation { get; set; }

        public string EnumValueName { get; set; }

        public string EnumValue { get; set; }

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

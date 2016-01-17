using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtRaw : RtNode
    {
        public string RawContent { get; set; }

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

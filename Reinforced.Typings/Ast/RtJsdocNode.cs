using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtJsdocNode : RtNode
    {
        public string Description { get; set; }

        public List<Tuple<DocTag, string>> TagToDescription { get; private set; }

        public RtJsdocNode()
        {
            TagToDescription = new List<Tuple<DocTag, string>>();
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

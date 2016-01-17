using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtIdentifier : RtNode
    {
        public RtIdentifier()
        {
        }

        public RtIdentifier(string identifierName)
        {
            IdentifierName = identifierName;
        }

        public string IdentifierName { get; set; }

        public bool IsNullable { get; set; }

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

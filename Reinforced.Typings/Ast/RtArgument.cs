using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtArgument : RtNode
    {
        public RtIdentifier Identifier { get; set; }

        public string DefaultValue { get; set; }

        public bool IsVariableParameters { get; set; }

        public RtTypeName Type { get; set; }

        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Identifier;
                yield return Type;
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

using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtFuncion : RtMember
    {
        public RtIdentifier Identifier { get; set; }

        public RtTypeName ReturnType { get; set; }

        public List<RtArgument> Arguments { get; set; }

        public RtRaw Body { get; set; }

        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Identifier;
                yield return ReturnType;
                foreach (var rtArgument in Arguments)
                {
                    yield return rtArgument;
                }
                if (Body != null) yield return Body;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public class RtConstructor : RtMember
    {
        public List<RtArgument> Arguments { get; set; }

        public List<string> SuperCallParameters { get; private set; }

        public bool NeedsSuperCall { get; set; }

        public RtConstructor()
        {
            Arguments = new List<RtArgument>();
            SuperCallParameters = new List<string>();
        }

        public RtRaw Body { get; set; }

        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtArgument in Arguments)
                {
                    yield return rtArgument;
                }
                yield return Body;
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

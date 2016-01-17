using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtDelegateType : RtTypeName
    {
        private readonly RtArgument[] _arguments;

        public RtArgument[] Arguments
        {
            get { return _arguments; }
        }

        public RtTypeName Result { get; private set; }

        public RtDelegateType(RtArgument[] arguments, RtTypeName result)
        {
            _arguments = arguments;
            Result = result;
        }

        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtArgument in Arguments)
                {
                    yield return rtArgument;
                }
                yield return Result;
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

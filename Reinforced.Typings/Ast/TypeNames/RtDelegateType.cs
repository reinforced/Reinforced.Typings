using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for delegate type
    /// </summary>
    public sealed class RtDelegateType : RtTypeName
    {
        private readonly RtArgument[] _arguments;

        /// <summary>
        /// Consumed arguments
        /// </summary>
        public RtArgument[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Returning result
        /// </summary>
        public RtTypeName Result { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        /// <param name="arguments">Delegate parameters</param>
        /// <param name="result">Delegate result type</param>
        public RtDelegateType(RtArgument[] arguments, RtTypeName result)
        {
            _arguments = arguments;
            Result = result;
        }

        /// <summary>
        /// Child nodes
        /// </summary>
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

        /// <summary>
        /// Visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Typed visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0}) => {1}", String.Join(", ", Arguments.AsEnumerable()), Result);
        }
    }
}

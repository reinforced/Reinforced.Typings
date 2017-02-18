using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript member function
    /// </summary>
    public class RtFuncion : RtMember
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtFuncion()
        {
            Arguments = new List<RtArgument>();
        }

        /// <summary>
        /// Function name
        /// </summary>
        public RtIdentifier Identifier { get; set; }

        /// <summary>
        /// Function return type
        /// </summary>
        public RtTypeName ReturnType { get; set; }

        /// <summary>
        /// Function parameters
        /// </summary>
        public List<RtArgument> Arguments { get; private set; }

        /// <summary>
        /// Function body (supplied as raw text)
        /// </summary>
        public RtRaw Body { get; set; }

        /// <summary>
        /// Child nodes
        /// </summary>
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
    }
}

using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript member function
    /// </summary>
    public class RtFunction : RtMember, IDecoratable
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtFunction()
        {
            Arguments = new List<RtArgument>();
            Decorators = new List<RtDecorator>();
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

        /// <inheritdoc />
        public List<RtDecorator> Decorators { get; private set; }

        /// <summary>
        /// Function body (supplied as raw text)
        /// </summary>
        public RtRaw Body { get; set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtDecorator in Decorators)
                {
                    yield return rtDecorator;
                }
                yield return Identifier;
                yield return ReturnType;
                foreach (var rtArgument in Arguments)
                {
                    yield return rtArgument;
                }
                if (Body != null) yield return Body;
            }
        }

        /// <inheritdoc />
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for class constructor
    /// </summary>
    public class RtConstructor : RtMember
    {
        /// <summary>
        /// Constructor parameters
        /// </summary>
        public List<RtArgument> Arguments { get; set; }

        /// <summary>
        /// Array of arguments to be substitute to super(...) call
        /// </summary>
        public List<string> SuperCallParameters { get; private set; }

        /// <summary>
        /// When true, super(...) call will be generated. Otherwise will not
        /// </summary>
        public bool NeedsSuperCall { get; set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtConstructor()
        {
            Arguments = new List<RtArgument>();
            SuperCallParameters = new List<string>();
        }

        /// <summary>
        /// Implementation body (raw content)
        /// </summary>
        public RtRaw Body { get; set; }

        /// <inheritdoc />
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

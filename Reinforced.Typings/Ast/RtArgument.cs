using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for method parameter
    /// </summary>
    public class RtArgument : RtNode
    {
        /// <summary>
        /// Parameter identifier
        /// </summary>
        public RtIdentifier Identifier { get; set; }

        /// <summary>
        /// Default value (raw typescript expression)
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Is this parameter represents variable method parameterss
        /// </summary>
        public bool IsVariableParameters { get; set; }

        /// <summary>
        /// Argument type
        /// </summary>
        public RtTypeName Type { get; set; }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Identifier;
                yield return Type;
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

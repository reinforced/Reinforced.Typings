using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Base Reinforced.Typings AST node
    /// </summary>
    public abstract class RtNode
    {
        /// <summary>
        /// Child nodes
        /// </summary>
        public abstract IEnumerable<RtNode> Children { get; }

        /// <summary>
        /// Visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public abstract void Accept(IRtVisitor visitor);

        /// <summary>
        /// Typed visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public abstract void Accept<T>(IRtVisitor<T> visitor);

    }
}

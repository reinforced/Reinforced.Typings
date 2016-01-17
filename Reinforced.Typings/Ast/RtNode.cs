using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Base Reinforced.Typings AST node
    /// </summary>
    public abstract class RtNode
    {
        
        public abstract IEnumerable<RtNode> Children { get; }

        public abstract void Accept(IRtVisitor visitor);

        public abstract void Accept<T>(IRtVisitor<T> visitor);

    }
}

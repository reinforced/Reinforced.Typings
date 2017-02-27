using System.Collections.Generic;

namespace Reinforced.Typings.Ast.Dependency
{
    /// <summary>
    /// AST node for TS reference exposed as comment
    /// </summary>
    public class RtReference : RtNode
    {
        /// <summary>
        /// File to reference
        /// </summary>
        public string Path { get; set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children { get { yield break; } }

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

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("///<reference path='{0}' />", Path);
        }
    }
}

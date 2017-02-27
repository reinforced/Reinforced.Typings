using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for identifier name
    /// </summary>
    public class RtIdentifier : RtNode
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtIdentifier()
        {
        }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        /// <param name="identifierName">identifier name</param>
        public RtIdentifier(string identifierName)
        {
            IdentifierName = identifierName;
        }

        /// <summary>
        /// Identifier name
        /// </summary>
        public string IdentifierName { get; set; }

        /// <summary>
        /// Is current identifier nullable
        /// </summary>
        public bool IsNullable { get; set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
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

        /// <inheritdoc />
        public override string ToString()
        {
            return IdentifierName + (IsNullable?"?":String.Empty);
        }
    }
}

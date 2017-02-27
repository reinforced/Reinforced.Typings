using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for array type
    /// </summary>
    public sealed class RtArrayType : RtTypeName
    {
        /// <summary>
        /// Array element type
        /// </summary>
        public RtTypeName ElementType { get; private set; }

        /// <summary>
        /// Constructs array type from existing type
        /// </summary>
        /// <param name="elementType"></param>
        public RtArrayType(RtTypeName elementType)
        {
            ElementType = elementType;
        }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get { yield return ElementType; }
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
            return String.Format("{0}[]",ElementType);
        }
    }
}

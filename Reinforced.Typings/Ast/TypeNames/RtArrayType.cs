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

        private bool Equals(RtArrayType other)
        {
            return Equals(ElementType, other.ElementType);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RtArrayType other && Equals(other);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (ElementType != null ? ElementType.GetHashCode() : 0);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtArrayType" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RtArrayType left, RtArrayType right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtArrayType" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RtArrayType left, RtArrayType right)
        {
            return !Equals(left, right);
        }
    }
}

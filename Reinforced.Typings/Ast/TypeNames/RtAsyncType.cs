using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for async return types of type "Promise".
    /// </summary>
    /// <remarks>With TypeScript, "Promise" use "generics" to define the resulting type of the "Promise". This is
    /// defined by a nested <see cref="TypeNameOfAsync"/></remarks>
    public sealed class RtAsyncType : RtTypeName
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtAsyncType(RtTypeName nestedType)
            : this()
        {
            TypeNameOfAsync = nestedType;
        }

        /// <summary>
        /// Type name
        /// </summary>
        public RtTypeName TypeNameOfAsync { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtAsyncType()
        {
        }

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
            return $"Promise<{TypeNameOfAsync?.ToString() ?? "void"}>";
        }

        private bool Equals(RtAsyncType other)
        {
            return Equals(TypeNameOfAsync, other.TypeNameOfAsync);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RtAsyncType other && Equals(other);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (TypeNameOfAsync != null ? TypeNameOfAsync.GetHashCode() : 0);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtAsyncType" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RtAsyncType left, RtAsyncType right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtAsyncType" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RtAsyncType left, RtAsyncType right)
        {
            return !Equals(left, right);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for simple type name
    /// </summary>
    public sealed class RtSimpleTypeName : RtTypeName
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtSimpleTypeName(RtTypeName[] genericArguments, string ns, string typeName)
        {
            _genericArguments = genericArguments;
            Prefix = ns;
            TypeName = typeName;
        }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtSimpleTypeName(string typeName)
            : this()
        {
            TypeName = typeName;
        }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtSimpleTypeName(string typeName, params RtTypeName[] genericArguments)
        {
            TypeName = typeName;
            if (genericArguments == null) genericArguments = new RtTypeName[0];
            _genericArguments = genericArguments;
        }

        private readonly RtTypeName[] _genericArguments;

        /// <summary>
        /// Type name generic arguments
        /// </summary>
        public RtTypeName[] GenericArguments { get { return _genericArguments; } }

        /// <summary>
        /// Type namespace
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// <c>true</c> if the <see cref="Prefix"/> is not empty.
        /// </summary>
        public bool HasPrefix => !string.IsNullOrEmpty(Prefix);

        /// <summary>
        /// Type name
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtSimpleTypeName()
        {
            _genericArguments = new RtTypeName[0];
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
            string generics = _genericArguments.Length > 0 ? "<" + String.Join(",", _genericArguments.AsEnumerable()) + ">" : null;
            var result = String.Concat(TypeName, generics);
            if (HasPrefix)
            {
                result =  Prefix + "." + result;
            }
            return result;
        }

        private bool Equals(RtSimpleTypeName other)
        {
            return Prefix == other.Prefix && TypeName == other.TypeName;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RtSimpleTypeName other && Equals(other);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_genericArguments != null ? _genericArguments.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Prefix != null ? Prefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TypeName != null ? TypeName.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtSimpleTypeName" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RtSimpleTypeName left, RtSimpleTypeName right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtSimpleTypeName" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RtSimpleTypeName left, RtSimpleTypeName right)
        {
            return !Equals(left, right);
        }
    }
}

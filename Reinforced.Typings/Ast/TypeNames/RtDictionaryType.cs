using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for Dictionary type
    /// </summary>
    public sealed class RtDictionaryType : RtTypeName
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtDictionaryType()
        {

        }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        /// <param name="keySimpleType">Type for dictionary key</param>
        /// <param name="valueSimpleType">Type for disctionary value</param>
        public RtDictionaryType(RtTypeName keySimpleType, RtTypeName valueSimpleType)
            : this(keySimpleType, valueSimpleType, false)
        {
        }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        /// <param name="keySimpleType">Type for dictionary key</param>
        /// <param name="valueSimpleType">Type for disctionary value</param>
        /// <param name="isKeyEnum">A flag specifying whether the key is an enum type.</param>
        public RtDictionaryType(RtTypeName keySimpleType, RtTypeName valueSimpleType, bool isKeyEnum)
        {
            KeyType = keySimpleType;
            ValueType = valueSimpleType;
            IsKeyEnum = isKeyEnum;
        }

        /// <summary>
        /// Type for dictionary key
        /// </summary>
        public RtTypeName KeyType { get; }

        /// <summary>
        /// Type for disctionary value
        /// </summary>
        public RtTypeName ValueType { get; }

        /// <summary>
        /// A flag indicating whether the key is an enum type, and a mapped type should be generated.
        /// </summary>
        public bool IsKeyEnum { get; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return KeyType;
                yield return ValueType;
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

        /// <inheritdoc />
        public override string ToString()
        {
            string keyTypeSpec = IsKeyEnum ? " in " : ":";
            return $"{{ [key{keyTypeSpec}{KeyType}]: {ValueType} }}";
        }

        private bool Equals(RtDictionaryType other)
        {
            return Equals(KeyType, other.KeyType) && Equals(ValueType, other.ValueType) && IsKeyEnum == other.IsKeyEnum;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RtDictionaryType other && Equals(other);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (KeyType != null ? KeyType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueType != null ? ValueType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsKeyEnum.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtDictionaryType" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RtDictionaryType left, RtDictionaryType right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Reinforced.Typings.Ast.TypeNames.RtDictionaryType" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RtDictionaryType left, RtDictionaryType right)
        {
            return !Equals(left, right);
        }
    }
}

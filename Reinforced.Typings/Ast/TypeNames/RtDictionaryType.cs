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
        public RtTypeName KeyType { get; private set; }

        /// <summary>
        /// Type for disctionary value
        /// </summary>
        public RtTypeName ValueType { get; private set; }

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
    }
}

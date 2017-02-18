using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for Dictionary type
    /// </summary>
    public class RtDictionaryType : RtTypeName
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
        public RtDictionaryType(string keySimpleType, string valueSimpleType)
        {
            KeyType = new RtSimpleTypeName(keySimpleType);
            ValueType = new RtSimpleTypeName(valueSimpleType);
        }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        /// <param name="keySimpleType">Type for dictionary key</param>
        /// <param name="valueSimpleType">Type for disctionary value</param>
        public RtDictionaryType(RtTypeName keySimpleType, RtTypeName valueSimpleType)
        {
            KeyType = keySimpleType;
            ValueType = valueSimpleType;
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
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return KeyType;
                yield return ValueType;
            }
        }

        /// <summary>
        /// Visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Typed visitor acceptance
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{{ [key: {0}]: {1} }}", KeyType, ValueType);
        }
    }
}

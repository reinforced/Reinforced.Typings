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
            if (!string.IsNullOrEmpty(Prefix))
            {
                result =  Prefix + "." + result;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for simple type name
    /// </summary>
    public class RtSimpleTypeName : RtTypeName
    {
        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtSimpleTypeName(RtTypeName[] genericArguments, string ns, string typeName)
        {
            _genericArguments = genericArguments;
            Namespace = ns;
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
        public RtSimpleTypeName(string typeName, RtTypeName[] genericArguments)
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
        public string Namespace { get; set; }

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

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
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
            string generics = _genericArguments.Length > 0 ? "<" + String.Join(",", _genericArguments.AsEnumerable()) + ">" : null;
            return String.Concat(TypeName, generics);
        }
    }
}

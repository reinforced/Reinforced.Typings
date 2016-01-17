using System;
using System.Collections.Generic;
using System.Linq;

namespace Reinforced.Typings.Ast
{
    public class RtSimpleTypeName : RtTypeName
    {
        public RtSimpleTypeName(RtTypeName[] genericArguments, string ns, string typeName)
        {
            _genericArguments = genericArguments;
            Namespace = ns;
            TypeName = typeName;
        }

        public RtSimpleTypeName(string typeName)
            : this()
        {
            TypeName = typeName;
        }

        private readonly RtTypeName[] _genericArguments;

        public RtTypeName[] GenericArguments { get { return _genericArguments; } }

        public RtSimpleTypeName(string typeName, RtTypeName[] genericArguments)
        {
            TypeName = typeName;
            if (genericArguments == null) genericArguments = new RtTypeName[0];
            _genericArguments = genericArguments;
        }

        public string Namespace { get; set; }

        public string TypeName { get; private set; }

        public RtSimpleTypeName()
        {
            _genericArguments = new RtTypeName[0];
        }
        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
        }

        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            string generics = _genericArguments.Length > 0 ? "<" + String.Join(",", _genericArguments.AsEnumerable()) + ">" : null;
            return String.Concat(TypeName, generics);
        }
    }
}

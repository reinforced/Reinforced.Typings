using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast.TypeNames
{
    /// <summary>
    /// AST node for TypeScript tuple type
    /// </summary>
    public class RtTuple : RtTypeName
    {
        public RtTuple()
        {
            TupleTypes = new List<RtTypeName>();
        }

        public RtTuple(IEnumerable<RtTypeName> tupleTypes)
        {
            TupleTypes = new List<RtTypeName>(tupleTypes);
        }

        public RtTuple(params RtTypeName[] tupleTypes)
        {
            TupleTypes = new List<RtTypeName>(tupleTypes);
        }

        /// <summary>
        /// All types that must participate tuple
        /// </summary>
        public List<RtTypeName> TupleTypes { get; private set; }

        /// <summary>
        /// Children
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get
            {
                foreach (var rtTypeName in TupleTypes)
                {
                    yield return rtTypeName;
                }
            }
        }

        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

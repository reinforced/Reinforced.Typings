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
        /// <summary>
        /// Constructs new RtTuple
        /// </summary>
        public RtTuple()
        {
            TupleTypes = new List<RtTypeName>();
        }

        /// <summary>
        /// Constructs new RtTuple with specified type paranmeters
        /// </summary>
        /// <param name="tupleTypes">Types for tuple</param>
        public RtTuple(IEnumerable<RtTypeName> tupleTypes)
        {
            TupleTypes = new List<RtTypeName>(tupleTypes);
        }

        /// <summary>
        /// Constructs new RtTuple with specified type paranmeters
        /// </summary>
        /// <param name="tupleTypes">Types for tuple</param>
        public RtTuple(params RtTypeName[] tupleTypes)
        {
            TupleTypes = new List<RtTypeName>(tupleTypes);
        }

        /// <summary>
        /// All types that must participate tuple
        /// </summary>
        public List<RtTypeName> TupleTypes { get; private set; }

        /// <inheritdoc />
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
    }
}

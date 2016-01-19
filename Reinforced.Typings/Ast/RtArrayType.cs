using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for array type
    /// </summary>
    public class RtArrayType : RtTypeName
    {
        /// <summary>
        /// Array element type
        /// </summary>
        public RtTypeName ElementType { get; private set; }

        /// <summary>
        /// Constructs array type from existing type
        /// </summary>
        /// <param name="elementType"></param>
        public RtArrayType(RtTypeName elementType)
        {
            ElementType = elementType;
        }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get { yield return ElementType; }
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
        public override string ToString()
        {
            return String.Format("{0}[]",ElementType);
        }
    }
}

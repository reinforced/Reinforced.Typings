using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript enumeration
    /// </summary>
    public class RtEnum : RtCompilationUnit
    {
        /// <summary>
        /// JSDOC
        /// </summary>
        public RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Enum name
        /// </summary>
        public RtSimpleTypeName EnumName { get; set; }

        /// <summary>
        /// Enum values
        /// </summary>
        public List<RtEnumValue> Values { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtEnum()
        {
            Values = new List<RtEnumValue>();
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
    }
}

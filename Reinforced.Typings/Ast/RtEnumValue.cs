using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript enumeration value
    /// </summary>
    public class RtEnumValue : RtNode
    {
        /// <summary>
        /// JSDOC
        /// </summary>
        public RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Value name
        /// </summary>
        public string EnumValueName { get; set; }

        /// <summary>
        /// Value value
        /// </summary>
        public string EnumValue { get; set; }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Syntax node for TS decorator
    /// </summary>
    public class RtDecorator : RtNode
    {
        /// <summary>
        /// Decorator name (everything that must follow after "@")
        /// </summary>
        public string Decorator { get; private set; }

        /// <summary>
        /// Order of appearence
        /// </summary>
        public double Order { get; set; }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children { get {yield break;} }

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

        public RtDecorator(string decorator, double order = 0)
        {
            Decorator = decorator;
            Order = order;
        }
    }
}

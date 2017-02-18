using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast
{
    public class RtReference : RtNode
    {
        public string Path { get; set; }

        /// <summary>
        /// Children
        /// </summary>
        public override IEnumerable<RtNode> Children { get { yield break; } }

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
        /// <typeparam name="T">Visitor type</typeparam>
        /// <param name="visitor">Typed visitor</param>
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Ast.Dependency
{
    /// <summary>
    /// Syntax node for import target
    /// </summary>
    public class RtImportTarget : RtNode
    {

        /// <summary>
        /// Import alias (everything that follows after "as" keyword)
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Flag when import target actually imports all (import *) 
        /// </summary>
        public bool ImportsAll { get; set; }

        /// <summary>
        /// Node children
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
        /// <param name="visitor">Visitor</param>
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

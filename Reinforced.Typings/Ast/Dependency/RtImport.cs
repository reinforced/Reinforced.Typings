using System.Collections.Generic;

namespace Reinforced.Typings.Ast.Dependency
{
    /// <summary>
    /// Import declaration
    /// </summary>
    public class RtImport : RtNode
    {
        /// <summary>
        /// Targets list
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Import source
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// When true, "from" part will be replaced with "= require('From')"
        /// </summary>
        public bool IsRequire { get; set; }

        /// <summary>
        /// Node children
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
        /// <typeparam name="T">Visitor type</typeparam>
        /// <param name="visitor">Visitor</param>
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

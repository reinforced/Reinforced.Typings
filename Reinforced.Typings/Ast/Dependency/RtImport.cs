using System.Collections.Generic;

namespace Reinforced.Typings.Ast.Dependency
{
    /// <summary>
    /// Import declaration
    /// </summary>
    public class RtImport : RtNode
    {
        public RtImport()
        {
            Targets = new List<RtImportTarget>();
        }

        /// <summary>
        /// Targets list
        /// </summary>
        public List<RtImportTarget> Targets { get; private set; }

        /// <summary>
        /// Import source
        /// </summary>
        public string From { get; set; }

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

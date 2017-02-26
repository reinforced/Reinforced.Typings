using System.Collections.Generic;

namespace Reinforced.Typings.Ast.Dependency
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

        public override string ToString()
        {
            return string.Format("///<reference path='{0}' />", Path);
        }
    }
}

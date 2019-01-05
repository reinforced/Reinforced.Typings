using System.Collections.Generic;

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

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children { get {yield break;} }

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

        /// <summary>
        /// Constructs new RtDecorator
        /// </summary>
        /// <param name="decorator">Decorator content</param>
        /// <param name="order">Decorator order</param>
        public RtDecorator(string decorator, double order = 0)
        {
            Decorator = decorator;
            Order = order;
        }
    }
}

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// Abstract AST node for class/interface member
    /// </summary>
    public abstract class RtMember : RtNode
    {
        /// <summary>
        /// JSDOC
        /// </summary>
        public RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Access modifier
        /// </summary>
        public AccessModifier? AccessModifier { get; set; }

        /// <summary>
        /// Is member static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Member order
        /// </summary>
        public double Order { get; set; }
    }
}

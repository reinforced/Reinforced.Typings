using System.Collections.Generic;

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
        /// Gets or sets line that will follow after member
        /// </summary>
        public string LineAfter { get; set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
        }

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
    }
}

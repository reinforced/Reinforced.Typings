using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for typeScript interface
    /// </summary>
    public class RtInterface : RtCompilationUnit, ITypeMember
    {
        /// <summary>
        /// Interface name
        /// </summary>
        public RtSimpleTypeName Name { get; set; }

        /// <summary>
        /// Implemented interfaces
        /// </summary>
        public List<RtSimpleTypeName> Implementees { get; private set; }

        /// <summary>
        /// JSDOC
        /// </summary>
        public RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Interface members
        /// </summary>
        public List<RtNode> Members { get; private set; }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Documentation;
                foreach (var rtNode in base.Children)
                {
                    yield return rtNode;
                }
                yield return Name;
                foreach (var rtSimpleTypeName in Implementees)
                {
                    yield return rtSimpleTypeName;
                }

                foreach (var rtMember in Members)
                {
                    yield return rtMember;
                }
            }
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

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtInterface()
        {
            Members = new List<RtNode>();
            Implementees = new List<RtSimpleTypeName>();
        }
    }
}

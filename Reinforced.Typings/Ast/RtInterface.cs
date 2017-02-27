using System.Collections.Generic;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for typeScript interface
    /// </summary>
    public class RtInterface : RtCompilationUnit, ITypeMember
    {
        /// <inheritdoc />
        public RtSimpleTypeName Name { get; set; }

        /// <inheritdoc />
        public List<RtSimpleTypeName> Implementees { get; private set; }

        /// <inheritdoc />
        public RtJsdocNode Documentation { get; set; }

        /// <inheritdoc />
        public List<RtNode> Members { get; private set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Documentation;
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
        /// Constructs new instance of AST node
        /// </summary>
        public RtInterface()
        {
            Members = new List<RtNode>();
            Implementees = new List<RtSimpleTypeName>();
        }
     
    }
}

using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript class
    /// </summary>
    public class RtClass : RtCompilationUnit, ITypeMember
    {
        /// <summary>
        /// JSDOC
        /// </summary>
        public RtJsdocNode Documentation { get; set; }

        /// <summary>
        /// Class name
        /// </summary>
        public RtSimpleTypeName Name { get; set; }

        /// <summary>
        /// Implemented interfaces list
        /// </summary>
        public List<RtSimpleTypeName> Implementees { get; private set; }

        /// <summary>
        /// Extended class
        /// </summary>
        public RtTypeName Extendee { get; set; }

        /// <summary>
        /// Class members
        /// </summary>
        public List<RtNode> Members { get; private set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtClass()
        {
            Members = new List<RtNode>();
            Implementees = new List<RtSimpleTypeName>();
        }

        /// <summary>
        /// Child nodes
        /// </summary>
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Documentation;
                yield return Name;
                foreach (var implementee in Implementees)
                {
                    yield return implementee;
                }
                yield return Extendee;
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
    }
}

using System.Collections.Generic;
using System.Linq;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Ast
{
    /// <summary>
    /// AST node for TypeScript class
    /// </summary>
    public class RtClass : RtCompilationUnit, ITypeMember, IDecoratable
    {
        /// <inheritdoc />
        public List<RtDecorator> Decorators { get; private set; }

        /// <inheritdoc />
        public RtJsdocNode Documentation { get; set; }

        /// <inheritdoc />
        public RtSimpleTypeName Name { get; set; }

        /// <inheritdoc />
        public List<RtSimpleTypeName> Implementees { get; private set; }

        /// <inheritdoc />
        public RtTypeName Extendee { get; set; }

        /// <inheritdoc />
        public List<RtNode> Members { get; private set; }        //todo this needs to be sorted to be cross-platform, it's returning different order for Mono
        
        ///  <inheritdoc />
        public bool Abstract { get; set; }

        /// <inheritdoc />
        public bool Inner { get; set; }

        /// <summary>
        /// Constructs new instance of AST node
        /// </summary>
        public RtClass()
        {
            Members = new List<RtNode>();
            Implementees = new List<RtSimpleTypeName>();
            Decorators = new List<RtDecorator>();
        }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get
            {
                yield return Documentation;

                foreach (var rtDecorator in Decorators)
                {
                    yield return rtDecorator;
                }
                
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

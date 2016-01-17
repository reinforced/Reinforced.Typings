using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtInterface : RtCompilationUnit, ITypeMember
    {
        public RtSimpleTypeName Name { get; set; }

        public bool NeedsExports { get; set; }

        public List<RtSimpleTypeName> Implementees { get; private set; }

        public RtJsdocNode Documentation { get; set; }

        public List<RtNode> Members { get; private set; }

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

        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        public RtInterface()
        {
            Members = new List<RtNode>();
            Implementees = new List<RtSimpleTypeName>();
        }
    }
}

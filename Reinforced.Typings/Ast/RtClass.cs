using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtClass : RtCompilationUnit, ITypeMember
    {
        public bool NeedsExports { get; set; }

        public bool IsStatic { get; set; }

        public RtJsdocNode Documentation { get; set; }

        public RtSimpleTypeName Name { get; set; }

        public List<RtSimpleTypeName> Implementees { get; private set; }

        public RtTypeName Extendee { get; set; }

        public List<RtMember> Members { get; private set; }

        public RtClass()
        {
            Members = new List<RtMember>();
            Implementees = new List<RtSimpleTypeName>();
        }

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

        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }
}

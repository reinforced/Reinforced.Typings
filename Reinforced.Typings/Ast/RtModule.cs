using System.Collections.Generic;

namespace Reinforced.Typings.Ast
{
    public class RtModule : RtNode
    {
        public bool IsAbstractModule { get; set; }

        public string NamespaceName { get; set; }

        public List<RtCompilationUnit> CompilationUnits { get; set; }

        public override IEnumerable<RtNode> Children
        {
            get { return CompilationUnits; }
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

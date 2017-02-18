using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;

namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtConstructor node)
        {
            if (node == null) return;
            AppendTabs();
            if (Context == WriterContext.Interface)
            {
                Write("new (");
            }
            else
            {
                Write("constructor (");
            }
            SequentialVisit(node.Arguments, ", ");
            WriteLine("); ");
        }
    }
}
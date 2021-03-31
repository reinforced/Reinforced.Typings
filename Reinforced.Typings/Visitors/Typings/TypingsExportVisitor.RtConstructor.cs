using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtConstructor node)
        {
            if (node == null) return;
            Visit(node.Documentation);
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
            WriteLine(");");

            if (!string.IsNullOrEmpty(node.LineAfter))
            {
                AppendTabs();
                Write(node.LineAfter);
                Br();
            }
        }
    }
}
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtFuncion node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            AppendTabs();
            if (Context != WriterContext.Interface) Modifiers(node);
            if (Context == WriterContext.Module) Write("export function ");
            Visit(node.Identifier);
            Write("(");
            SequentialVisit(node.Arguments, ", ");
            Write(") ");
            if (node.ReturnType != null)
            {
                Write(": ");
                Visit(node.ReturnType);
            }
            WriteLine(";");
        }
    }
}
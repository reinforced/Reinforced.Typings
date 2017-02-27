using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtField node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            AppendTabs();

            if (Context == WriterContext.Module) Write("export var ");
            if (Context == WriterContext.None) Write("declare var ");
            if (Context == WriterContext.Class) Modifiers(node);

            Visit(node.Identifier);
            Write(": ");
            Visit(node.Type);
            WriteLine(";");
        }
    }
}
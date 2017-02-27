using Reinforced.Typings.Ast;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtEnum node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            var prev = Context;
            Context = WriterContext.Enum;
            AppendTabs();
            if (node.Export) Write("export ");
            Write("enum ");
            Visit(node.EnumName);
            WriteLine(" { ");
            Tab();
            foreach (var rtEnumValue in node.Values)
            {
                Visit(rtEnumValue);
            }
            UnTab();
            AppendTabs();
            WriteLine("}");
            Context = prev;
        }
    }
}
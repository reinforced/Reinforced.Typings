using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;

#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtEnum node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            var prev = Context;
            Context = WriterContext.Enum;
            AppendTabs();
            if (node.Export) Write("declare ");
            if (node.IsConst) Write("const ");
            Write("enum ");
            Visit(node.EnumName);
            WriteLine(" { ");
            Tab();
            var arr = node.Values.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                Visit(arr[i]);
                if (i != arr.Length - 1) WriteLine(", ");
            }
            WriteLine(string.Empty);
            UnTab();
            AppendTabs();
            WriteLine("}");
            Context = prev;
        }
    }
}
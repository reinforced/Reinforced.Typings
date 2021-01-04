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
            if (node.IsConst) Write("const ");
            Write("enum ");
            Visit(node.EnumName);
            WriteLine(" {");
            Tab();
            var arr = node.Values.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                Visit(arr[i]);
                if (i != arr.Length - 1) WriteLine(",");
                if (!string.IsNullOrEmpty(arr[i].LineAfter))
                {
                    AppendTabs();
                    Write(arr[i].LineAfter);
                    Br();
                }
            }
            WriteLine(string.Empty);
            UnTab();
            AppendTabs();
            WriteLine("}");
            Context = prev;
        }
    }
}
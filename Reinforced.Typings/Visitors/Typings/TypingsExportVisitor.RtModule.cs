using System;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtNamespace node)
        {
            if (node == null) return;

            if (!node.IsAmbientNamespace)
            {
                Context = WriterContext.Module;
                AppendTabs();
                WriteLine(String.Format("declare module {0} {{", node.Name));
                Tab();
            }
            foreach (var rtCompilationUnit in node.CompilationUnits.OrderBy(c => c is RtCompilationUnit ? ((RtCompilationUnit)c).Order : 0))
            {
                Visit(rtCompilationUnit);
            }
            if (!node.IsAmbientNamespace)
            {
                Context = WriterContext.None;
                UnTab();
                AppendTabs();
                WriteLine("}");
            }
        }
    }
}

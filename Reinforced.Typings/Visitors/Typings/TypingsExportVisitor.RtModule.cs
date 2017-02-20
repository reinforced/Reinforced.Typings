using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;

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
                WriteLine(String.Format("declare module {0} {{", node.ModuleName));
                Tab();
            }
            foreach (var rtCompilationUnit in node.CompilationUnits)
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
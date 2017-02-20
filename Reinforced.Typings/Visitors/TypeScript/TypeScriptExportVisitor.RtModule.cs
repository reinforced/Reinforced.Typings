using System;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtNamespace node)
        {
            if (node == null) return;

            if (!node.IsAmbientNamespace)
            {
                Context = WriterContext.Module;
                AppendTabs();
                WriteLine(String.Format("module {0} {{", node.ModuleName));
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
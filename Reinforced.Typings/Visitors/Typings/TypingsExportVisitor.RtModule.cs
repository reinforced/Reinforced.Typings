using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;

namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
        public override void Visit(RtModule node)
        {
            if (node == null) return;

            if (!node.IsAmbientModule)
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
            if (!node.IsAmbientModule)
            {
                Context = WriterContext.None;
                UnTab();
                AppendTabs();
                WriteLine("}");
            }
        }
    }
}
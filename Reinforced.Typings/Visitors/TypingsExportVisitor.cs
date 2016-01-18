using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Visitors
{
    class TypingsExportVisitor : TypeScriptExportVisitor
    {
        public TypingsExportVisitor(TextWriter writer)
            : base(writer)
        {
        }

        public override void Visit(RtFuncion node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            if (Context != WriterContext.Interface) Modifiers(node);
            if (Context == WriterContext.Module) Write("export function ");
            Visit(node.Identifier);
            Write("(");
            SequentialVisit(node.Arguments, ", ");
            Write(") ");
            if (node.ReturnType != null)
            {
                Write(": "); Visit(node.ReturnType);
            }
            WriteLine(";");
        }

        public override void Visit(RtModule node)
        {
            if (node == null) return;

            if (!node.IsAbstractModule)
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
            if (!node.IsAbstractModule)
            {
                Context = WriterContext.None;
                UnTab();
                AppendTabs();
                WriteLine("}");
            }
        }

        public override void Visit(RtConstructor node)
        {
            if (node == null) return;
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
            Write("); ");

        }

        public override void Visit(RtClass node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            var prev = Context;
            Context = WriterContext.Class;
            AppendTabs();
            if (prev == WriterContext.Module) Write("export ");
            else Write("declare ");

            Write("class ");
            Visit(node.Name);
            if (node.Extendee != null)
            {
                Write("extends ");
                Visit(node.Extendee);
            }
            if (node.Implementees.Count > 0)
            {
                Write("implements ");
                SequentialVisit(node.Implementees, ", ");
            }
            Br(); AppendTabs();
            Write("{"); Br();
            Tab();
            var members = node.Members.OrderBy(c => c is RtConstructor ? 0 : 1);
            foreach (var rtMember in members)
            {
                AppendTabs(); Visit(rtMember);
            }
            UnTab();
            Write("}");
            Context = prev;
        }

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
            Write(";");
            Br();
        }
    }
}

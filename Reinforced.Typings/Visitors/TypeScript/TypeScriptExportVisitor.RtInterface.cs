using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtInterface node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            var prev = Context;
            Context = WriterContext.Interface;
            AppendTabs();
            if (node.Export) Write("export ");
            Write("interface ");
            Visit(node.Name);
            if (node.Implementees.Count > 0)
            {
                Write(" extends ");
                SequentialVisit(node.Implementees, ", ");
            }
            Br(); AppendTabs();
            Write("{"); Br();
            Tab();
            foreach (var rtMember in node.Members)
            {
                Visit(rtMember);
            }
            UnTab();
            AppendTabs(); WriteLine("}");
            Context = prev;
        }

    }
}

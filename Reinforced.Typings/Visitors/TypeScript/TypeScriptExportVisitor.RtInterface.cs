using System.Linq;
using Reinforced.Typings.Ast;
#pragma warning disable 1591
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
            foreach (var rtMember in node.Members.OrderBy(c=> c is RtMember ? ((RtMember) c).Order : (double?) null))
            {
                Visit(rtMember);
            }
            UnTab();
            AppendTabs(); WriteLine("}");
            Context = prev;
        }

    }
}

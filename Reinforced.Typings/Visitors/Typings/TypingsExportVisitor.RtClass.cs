using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Visitors.TypeScript;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.Typings
{
    partial class TypingsExportVisitor
    {
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
                Write(" extends ");
                Visit(node.Extendee);
            }
            if (node.Implementees.Count > 0)
            {
                Write(" implements ");
                SequentialVisit(node.Implementees, ", ");
            }
            Br();
            AppendTabs();
            Write("{");
            Br();
            Tab();
            var members = node.Members.OrderBy(c => c is RtConstructor ? 0 : 1);
            foreach (var rtMember in members)
            {
                Visit(rtMember);
            }
            UnTab();
            AppendTabs();
            WriteLine("}");
            Context = prev;
        }
    }
}
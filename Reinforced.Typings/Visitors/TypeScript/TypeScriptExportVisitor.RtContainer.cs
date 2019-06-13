using Reinforced.Typings.Ast;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtContainer node)
        {
            if (node == null) return;
            foreach (RtNode child in node.Children)
            {
                Visit(child);
            }
        }

    }
}

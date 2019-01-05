using Reinforced.Typings.Ast;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtField node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            AppendTabs();
            if (Context != WriterContext.Interface)
            {
                Decorators(node);
                Modifiers(node);
            }
            Visit(node.Identifier);
            Write(": ");
            Visit(node.Type);
            if (!string.IsNullOrEmpty(node.InitializationExpression))
            {
                Write(" = ");
                Write(node.InitializationExpression);
            }
            Write(";");
            Br();
        }
    }
}

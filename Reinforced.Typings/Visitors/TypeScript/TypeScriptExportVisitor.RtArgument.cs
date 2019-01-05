using Reinforced.Typings.Ast;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtArgument node)
        {
            if (node == null) return;
            Decorators(node);
            if (node.IsVariableParameters) Write("...");
            Visit(node.Identifier);
            Write(": ");
            Visit(node.Type);
            if (!string.IsNullOrEmpty(node.DefaultValue))
            {
                Write(" = ");
                Write(node.DefaultValue);
            }
        }

    }
}

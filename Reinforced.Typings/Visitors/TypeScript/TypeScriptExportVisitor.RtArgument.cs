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

            // if a default value is used, then the parameter may be omitted as the default value is in place.
            if (Context == WriterContext.Interface && !node.Identifier.IsNullable && !string.IsNullOrEmpty(node.DefaultValue))
            {
                Write("?");
            }

            Write(": ");
            Visit(node.Type);
            if (Context != WriterContext.Interface && !string.IsNullOrEmpty(node.DefaultValue))
            {
                Write(" = ");
                Write(node.DefaultValue);
            }
        }

    }
}

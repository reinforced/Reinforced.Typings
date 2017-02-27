using Reinforced.Typings.Ast.TypeNames;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types

        public override void Visit(RtDictionaryType node)
        {
            if (node == null) return;
            Write("{ [key:");
            Visit(node.KeyType);
            Write("]: ");
            Visit(node.ValueType);
            Write(" }");
        }

        #endregion
    }
}
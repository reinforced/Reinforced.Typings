using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types

        public override void Visit(RtArrayType node)
        {
            if (node == null) return;
            Visit(node.ElementType);
            Write("[]");
        }

        #endregion
    }
}
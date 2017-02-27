using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types

        public override void Visit(RtDelegateType node)
        {
            if (node == null) return;
            Write("(");
            SequentialVisit(node.Arguments, ", ");
            Write(") => ");
            Visit(node.Result);
        }

        #endregion
    }
}
using Reinforced.Typings.Ast.TypeNames;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types

        public override void Visit(RtAsyncType node)
        {
            Write("Promise<");

            if (node.TypeNameOfAsync != null) this.Visit(node.TypeNameOfAsync);
            else Write("void");

            Write(">");
        }

        #endregion
    }
}
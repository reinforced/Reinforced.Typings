using Reinforced.Typings.Ast.TypeNames;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types

        public override void Visit(RtSimpleTypeName node)
        {
            if (!string.IsNullOrEmpty(node.Prefix))
            {
                Write(node.Prefix);
                Write(".");
            }
            Write(node.TypeName);
            if (node.GenericArguments.Length > 0)
            {
                Write("<");
                SequentialVisit(node.GenericArguments, ", ");
                Write(">");
            }
        }

        #endregion
    }
}
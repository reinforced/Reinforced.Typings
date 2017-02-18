using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types

        public override void Visit(RtSimpleTypeName node)
        {
            if (!string.IsNullOrEmpty(node.Namespace))
            {
                Write(node.Namespace);
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
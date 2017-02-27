using Reinforced.Typings.Ast.TypeNames;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        #region Types
        public override void Visit(RtTuple node)
        {
            if (node==null) return;
            Write("[");
            SequentialVisit(node.TupleTypes,",");
            Write("]");
        }


        #endregion


    }
}
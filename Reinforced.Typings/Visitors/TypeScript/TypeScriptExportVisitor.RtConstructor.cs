using System;
using Reinforced.Typings.Ast;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtConstructor node)
        {
            if (node == null) return;
            if (Context == WriterContext.Interface) return;
            Visit(node.Documentation);
            AppendTabs();
            Write("constructor (");
            SequentialVisit(node.Arguments, ", ");
            Write(")");
            if (node.NeedsSuperCall && node.Body == null)
            {
                string ncp = string.Empty;
                if (node.SuperCallParameters != null) ncp = string.Join(", ", node.SuperCallParameters);
                node.Body = new RtRaw(String.Format("super({0});", ncp));
            }

            if (node.Body != null && !string.IsNullOrEmpty(node.Body.RawContent))
            {
                CodeBlock(node.Body);
            }
            else
            {
                EmptyBody(null);
            }

            if (!string.IsNullOrEmpty(node.LineAfter))
            {
                AppendTabs();
                Write(node.LineAfter);
                Br();
            }
        }
    }
}
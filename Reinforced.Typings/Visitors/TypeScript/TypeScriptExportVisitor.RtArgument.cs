using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

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

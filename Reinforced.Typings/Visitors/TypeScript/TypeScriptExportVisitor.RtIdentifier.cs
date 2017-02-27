using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;
#pragma warning disable 1591
namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtIdentifier node)
        {
            if (node == null) return;
            Write(node.IdentifierName);
            if (node.IsNullable) Write("?");
        }
    }
}

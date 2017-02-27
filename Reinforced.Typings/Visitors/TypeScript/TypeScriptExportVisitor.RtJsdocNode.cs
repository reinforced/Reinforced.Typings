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
        public override void Visit(RtJsdocNode node)
        {
            if (node == null) return;

            if (!node.Description.Contains("\n") && node.TagToDescription.Count == 0)
            {
                //handle single-line JSDOC
                AppendTabs(); Write("/** "); Write(node.Description); WriteLine(" */");
            }
            else
            {
                AppendTabs(); WriteLine("/**");
                if (!string.IsNullOrEmpty(node.Description))
                {
                    Summary(node.Description);
                }
                if (node.TagToDescription.Count > 0) DocLine();
                foreach (var tuple in node.TagToDescription)
                {
                    DocTag(tuple.Item1, tuple.Item2);
                }
                AppendTabs(); WriteLine("*/");
            }
        }

    }
}

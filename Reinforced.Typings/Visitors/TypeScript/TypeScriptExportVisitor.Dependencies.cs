using Reinforced.Typings.Ast.Dependency;

#pragma warning disable 1591

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtImport node)
        {
            var quote = node.UseDoubleQuotes ? "\"" : "'";

            Write("import ");
            if (node.Target != null)
            {
                Write(node.Target);
                Write(" ");
                if (node.IsRequire)
                {
                    WriteLine($"= require({quote}{node.From}{quote});");
                }
                else
                {
                    WriteLine($"from {quote}{node.From}{quote};");
                }
            }
            else
            {
                WriteLine($"{quote}{node.From}{quote};");
            }

        }

        public override void Visit(RtReference node)
        {
            WriteLine($"///<reference path=\"{node.Path}\"/>");
        }
    }
}
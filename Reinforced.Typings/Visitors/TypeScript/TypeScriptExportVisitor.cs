using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor : TextExportingVisitor
    {
        protected WriterContext Context { get; set; }

        public TypeScriptExportVisitor(TextWriter writer, string tabulation)
            : base(writer,tabulation)
        {
            Context = WriterContext.None;
        }

        protected void Modifiers(RtMember member)
        {
            if (member.AccessModifier != null)
            {
                Write(member.AccessModifier.Value.ToModifierText());
                Write(" ");
            }
            if (member.IsStatic)
            {
                Write("static ");
            }
        }

        #region Documentation
        protected void DocTag(DocTag tag, string value)
        {
            var tagText = tag.Tagname();
            if (string.IsNullOrEmpty(value))
            {
                DocLine(tagText);
            }
            else
            {
                value = value.Replace("\r", null).Replace("\n", null);
                DocLine(String.Format("{0} {1}", tagText, value));
            }
        }
        protected void DocLine(string line = null)
        {
            if (string.IsNullOrEmpty(line))
            {
                AppendTabs(); WriteLine("*");
            }
            else
            {
                AppendTabs(); WriteLine(String.Format("* {0}", line));
            }
        }

        private void Summary(string summary)
        {
            if (string.IsNullOrEmpty(summary)) return;
            var summaryLines = summary.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var summaryLine in summaryLines)
            {
                DocLine(summaryLine);
            }
        }
        #endregion

        private void EmptyBody(RtTypeName returnType)
        {
            if (returnType == null || returnType.IsVoid())
            {
                WriteLine(" { } ");
            }
            else
            {
                CodeBlock("return null;");
            }
        }

        private void CodeBlock(string content)
        {
            Br();
            AppendTabs();
            WriteLine("{");
            Tab();
            WriteLines(content);
            UnTab();
            AppendTabs();
            WriteLine("}");
        }

        private void CodeBlock(RtRaw content)
        {
            Br();
            AppendTabs();
            WriteLine("{");
            Tab();
            Visit(content);
            UnTab();
            AppendTabs();
            WriteLine("}");
        }

        protected void SequentialVisit<T>(IEnumerable<T> nodes, string separator)
            where T : RtNode
        {
            var n = nodes.ToArray();
            for (int index = 0; index < n.Length; index++)
            {
                var rtArgument = n[index];
                Visit(rtArgument);
                if (index < n.Length - 1) Write(separator);
            }
        }

        public override void Visit(RtImport node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(RtDecorator node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(RtReference node)
        {
            throw new NotImplementedException();
        }
    }

    internal enum WriterContext
    {
        None,
        Class,
        Interface,
        Enum,
        Module
    }

    internal static class ExportExtensions
    {
        public static bool IsVoid(this RtTypeName typeName)
        {
            if (typeName == null) return true;
            var tn = typeName as RtSimpleTypeName;
            if (tn == null) return false;
            return tn.TypeName == "void";
        }
    }

}

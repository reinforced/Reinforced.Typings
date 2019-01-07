using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Ast.TypeNames;
#pragma warning disable 1591

namespace Reinforced.Typings.Visitors.TypeScript
{

    public partial class TypeScriptExportVisitor : TextExportingVisitor
    {
        protected WriterContext Context { get; set; }

        /// <summary>
        /// Gets whether it is needed to reorder class/interface members alphabetically when exporting
        /// </summary>
        protected bool ReorderMembers { get; private set; }

        public TypeScriptExportVisitor(TextWriter writer, string tabulation, bool reorderMembers)
            : base(writer, tabulation)
        {
            ReorderMembers = reorderMembers;
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

        protected void Decorators(IDecoratable member)
        {
            foreach (var memberDecorator in member.Decorators.OrderBy(c => c.Order))
            {
                Visit(memberDecorator);
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

        public override void Visit(RtDecorator node)
        {
            if (node == null) return;
            Write("@");
            Write(node.Decorator);
            Write(" ");
        }

        private bool IsKnownMember(RtNode n)
        {
            if (n is RtConstructor) return true;
            if (n is RtField) return true;
            if (n is RtFuncion) return true;
            return false;
        }

        private IEnumerable<RtNode> DoNodesOrder(List<RtNode> nodes)
        {
            var constructors = nodes.Where(d => d is RtConstructor).OfType<RtConstructor>();
            var fields = nodes.Where(d => d is RtField).OfType<RtField>().OrderBy(d => d.Identifier.IdentifierName);
            var methods = nodes.Where(d => d is RtFuncion).OfType<RtFuncion>().OrderBy(d => d.Identifier.IdentifierName);
            var rest = nodes.Where(d => !IsKnownMember(d));

            return constructors.Cast<RtNode>()
                    .Union(fields)
                    .Union(methods)
                    .Union(rest);
        }

        protected IEnumerable<RtNode> DoSortClassMembers(List<RtNode> nodes)
        {
            if (ReorderMembers)
            {
                return DoNodesOrder(nodes);
            }
            else
            {
                return nodes.OrderBy(c =>
                    c is RtConstructor ? int.MinValue : (c is RtMember ? ((RtMember)c).Order : (double?)null));
            }
        }

        protected IEnumerable<RtNode> DoSortInterfaceMembers(List<RtNode> nodes)
        {
            if (ReorderMembers)
            {
                return DoNodesOrder(nodes);
            }
            else
            {
                return nodes.OrderBy(c => c is RtMember ? ((RtMember)c).Order : (double?)null);
            }
        }
    }

    public enum WriterContext
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

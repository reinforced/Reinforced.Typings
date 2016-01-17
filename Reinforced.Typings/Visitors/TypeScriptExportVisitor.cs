using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Visitors
{
    public class TypeScriptExportVisitor : TextExportingVisitor
    {
        private WriterContext _context;

        public TypeScriptExportVisitor(TextWriter writer)
            : base(writer)
        {
            _context = WriterContext.None;
        }

        private void Modifiers(RtMember member)
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
                DocLine(String.Format("{0} {1}",tagText,value));
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

        public override void Visit(RtField node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            AppendTabs();

            if (_context != WriterContext.Interface) Modifiers(node);
            Visit(node.Identifier);
            Write(": ");
            Visit(node.Type);
            Write(";");
            Br();
        }

        public override void Visit(RtInterface node)
        {
            if (node==null) return;
            Visit(node.Documentation);
            var prev = _context;
            _context = WriterContext.Interface;
            AppendTabs();
            if (prev == WriterContext.Module) Write("export ");
            Write("interface ");
            Visit(node.Name);
            if (node.Implementees.Count > 0)
            {
                Write("extends ");
                SequentialVisit(node.Implementees,", ");
            }
            Br(); AppendTabs();
            Write("{"); Br();
            Tab();
            foreach (var rtMember in node.Members)
            {
                AppendTabs(); Visit(rtMember);
            }
            UnTab();
            Write("}");
            _context = prev;
        }

        public override void Visit(RtFuncion node)
        {
            if (node==null) return;
            Visit(node.Documentation);
            if (_context != WriterContext.Interface) Modifiers(node);
            Visit(node.Identifier);
            Write("(");
            SequentialVisit(node.Arguments,", ");
            Write(") ");
            if (node.ReturnType != null)
            {
                Write(": "); Visit(node.ReturnType);
            }

            if (_context == WriterContext.Interface)
            {
                WriteLine(";");
            }
            else
            {
                if (node.Body != null && !string.IsNullOrEmpty(node.Body.RawContent))
                {
                    CodeBlock(node.Body);
                }
                else
                {
                    EmptyBody(node.ReturnType);
                }
            }
        }

        private void EmptyBody(RtTypeName returnType)
        {
            if (returnType == null || returnType.IsVoid())
            {
                Write(" { } ");
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

        public override void Visit(RtArgument node)
        {
            if (node==null) return;
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

        public override void Visit(RtClass node)
        {
            if (node==null) return;
            Visit(node.Documentation);
            var prev = _context;
             _context = WriterContext.Class;
            //throw new NotImplementedException();
            _context = prev;
        }

        public override void Visit(RtIdentifier node)
        {
            if (node==null) return;
            Write(node.IdentifierName);
            if (node.IsNullable) Write("?");
        }

        public override void Visit(RtRaw node)
        {
            if (node==null) return;
            if (string.IsNullOrEmpty(node.RawContent)) return;
            WriteLines(node.RawContent);
        }

        public override void Visit(RtJsdocNode node)
        {
            if (node==null) return;
            AppendTabs(); WriteLine("/**");
            if (!string.IsNullOrEmpty(node.Description))
            {
                Summary(node.Description);
                DocLine();
            }
            foreach (var tuple in node.TagToDescription)
            {
                DocTag(tuple.Item1,tuple.Item2);
            }
            AppendTabs(); WriteLine("*/");
        }

        public override void Visit(RtModule node)
        {
            if (node==null) return;
            
            if (!node.IsAbstractModule)
            {
                _context = WriterContext.Module;
                AppendTabs();
                WriteLine(String.Format("module {0} {{",node.NamespaceName));
                Tab();
            }
            foreach (var rtCompilationUnit in node.CompilationUnits)
            {
                Visit(rtCompilationUnit);
            }
            if (!node.IsAbstractModule)
            {
                _context = WriterContext.None;
                UnTab();
                AppendTabs();
                WriteLine("}");
            }
        }

        public override void Visit(RtEnumValue node)
        {
            if (node==null) return;
            Visit(node.Documentation);
            AppendTabs();
            Write(node.EnumValueName);
            if (!string.IsNullOrEmpty(node.EnumValue))
            {
                Write(" = "); Write(node.EnumValue);
            }
            WriteLine(", ");
        }

        public override void Visit(RtEnum node)
        {
            if (node==null) return;
            Visit(node.Documentation);
            var prev = _context;
            _context = WriterContext.Interface;
            AppendTabs(); 
            if (prev == WriterContext.Module) Write("export ");
            Write("enum ");
            Visit(node.EnumName);
            WriteLine(" { ");
            Tab();
            foreach (var rtEnumValue in node.Values)
            {
                Visit(rtEnumValue);
            }
            UnTab();
            WriteLine("}");
            _context = prev;
        }

        #region Types
        public override void Visit(RtDelegateType node)
        {
            if (node == null) return;
            Write("(");
            SequentialVisit(node.Arguments,", ");
            Write(") => ");
            Visit(node.Result);
        }

        private void SequentialVisit<T>(IEnumerable<T> nodes, string separator)
            where T: RtNode
        {
            var n = nodes.ToArray();
           for (int index = 0; index < n.Length; index++)
            {
                var rtArgument = n[index];
                Visit(rtArgument);
                if (index < n.Length - 1) Write(separator);
            } 
        }

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
                SequentialVisit(node.GenericArguments,", ");
                Write(">");
            }
        }

        public override void Visit(RtDictionaryType node)
        {
            if (node == null) return;
            Write("{ [key:"); Visit(node.KeyType); Write("]: "); Visit(node.ValueType); Write(" }");
        }

        public override void Visit(RtArrayType node)
        {
            if (node == null) return;
            Visit(node.ElementType); Write("[]");
        }
        #endregion

        public override void Visit(RtConstructor node)
        {
            if (node==null) return;
            AppendTabs(); Write("constructor (");
            SequentialVisit(node.Arguments,", ");
            Write(")");
            if (node.NeedsSuperCall)
            {
                string ncp = string.Empty;
                if (node.SuperCallParameters != null) ncp = string.Join(", ", node.SuperCallParameters);
                CodeBlock(String.Format("super({0});", ncp));
            }
            else
            {
                EmptyBody(null);
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

    public static class ExportExtensions
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

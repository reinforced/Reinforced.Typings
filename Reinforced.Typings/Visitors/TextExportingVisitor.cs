using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Reinforced.Typings.Ast;

#pragma warning disable 1591

namespace Reinforced.Typings.Visitors
{
    public abstract class TextExportingVisitor : VisitorBase
    {
        private readonly TextWriter _writer;

        private int _tabsCount;
        private string _tabsLine;
        private readonly string _tabulation;
        protected TextWriter Writer { get { return _writer; } }

        /// <summary>
        /// Gets current export context
        /// </summary>
        protected ExportContext ExportContext { get; private set; }

        protected TextExportingVisitor(TextWriter writer, ExportContext exportContext)
        {
            _writer = writer;
            _tabulation = exportContext.Global.TabSymbol;
            ExportContext = exportContext;
        }

        private string TabLine(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(_tabulation);
            }
            return sb.ToString();
        }

        protected void Tab()
        {
            _tabsCount++;
            _tabsLine = TabLine(_tabsCount);
        }

        protected void UnTab()
        {
            if (_tabsCount > 0) _tabsCount--;
            _tabsLine = TabLine(_tabsCount);
        }

        protected void Indent()
        {
            AppendTabs();
        }

        protected void AppendTabs()
        {
            _writer.Write(_tabsLine);
        }

        public void Br()
        {
            _writer.WriteLine();
        }

        protected void WriteLines(string str)
        {
            var result = str.Split('\n');
            foreach (var s in result)
            {
                AppendTabs();
                _writer.WriteLine(s.Replace("\n", null).Replace("\r", null));
            }
        }

        protected void WriteLines(string format, params object[] args)
        {
            var formatted = string.Format(format, args);
            var result = formatted.Split('\n');
            foreach (var s in result)
            {
                AppendTabs();
                _writer.Write(s.Replace("\n", null).Replace("\r", null));
                _writer.WriteLine();
            }
        }

        protected void Write(string text)
        {
            _writer.Write(text);
        }
        protected void WriteLine(string text)
        {
            _writer.WriteLine(text);
        }

        public override void VisitFile(ExportedFile file)
        {
            bool hasReferences = false, hasImports = false;

            foreach (var rtReference in file.FinalReferences)
            {
                Visit(rtReference);
                hasReferences = true;
            }

            foreach (var rtImport in file.FinalImports)
            {
                Visit(rtImport);
                hasImports = true;
            }
            if (hasReferences || hasImports) Br();

            foreach (var fileNamespace in file.Namespaces)
            {
                Visit(fileNamespace);
            }

            foreach (var rtNode in file.CustomGenerators.SelectMany(g => g.Generate() ?? new List<RtNode>()))
            {
                Visit(rtNode);
            }
        }
    }
}

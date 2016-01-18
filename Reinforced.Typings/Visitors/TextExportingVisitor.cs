using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Visitors
{
    abstract class TextExportingVisitor : VisitorBase
    {
        private readonly TextWriter _writer;

        private int _tabsCount;
        private string _tabsLine;

        protected TextWriter Writer { get { return _writer; } }

        public TextExportingVisitor(TextWriter writer)
        {
            _writer = writer;
        }

        protected void Tab()
        {
            _tabsCount++;
            _tabsLine = new string('\t', _tabsCount);
        }

        protected void UnTab()
        {
            if (_tabsCount > 0) _tabsCount--;
            _tabsLine = new string('\t', _tabsCount);
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
    }
}

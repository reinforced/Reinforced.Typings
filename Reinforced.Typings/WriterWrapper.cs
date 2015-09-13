using System;
using System.IO;
#pragma warning disable 1591

namespace Reinforced.Typings
{
    public class WriterWrapper
    {
        private readonly TextWriter _writer;
        private int _tabsCount;
        private string _tabsLine;

        public WriterWrapper(TextWriter writer)
        {
            _writer = writer;
            _tabsLine = String.Empty;
        }

        private void AppendTabs()
        {
            _writer.Write(_tabsLine);
        }

        public void Tab()
        {
            _tabsCount++;
            _tabsLine = new string('\t', _tabsCount);
        }

        public void UnTab()
        {
            _tabsCount--;
            _tabsLine = new string('\t', _tabsCount);
        }
        public void WriteIndented(string str)
        {
            string[] result = str.Split('\n');
            foreach (var s in result)
            {
                AppendTabs();
                _writer.Write(s.Replace("\n",null).Replace("\r",null));
                _writer.WriteLine();
            }
            _writer.WriteLine();
        }
        public void WriteIndented(string format, params object[] args)
        {
            var formatted = string.Format(format, args);
            string[] result = formatted.Split('\n');
            foreach (var s in result)
            {
                AppendTabs();
                _writer.Write(s.Replace("\n", null).Replace("\r", null));
                _writer.WriteLine();
            }
            _writer.WriteLine();
        }
        public void Indent()
        {
            AppendTabs();
        }
        public void Write(bool value)
        {
            _writer.Write(value);
        }

        public void Write(int value)
        {
            _writer.Write(value);
        }

        public void Write(uint value)
        {
            _writer.Write(value);
        }

        public void Write(long value)
        {
            _writer.Write(value);
        }

        public void Write(ulong value)
        {
            _writer.Write(value);
        }

        public void Write(float value)
        {
            _writer.Write(value);
        }

        public void Write(double value)
        {
            _writer.Write(value);
        }

        public void Write(decimal value)
        {
            _writer.Write(value);
        }

        public void Write(object value)
        {
            _writer.Write(value);
        }

        public void Write(string format, object arg0)
        {
            _writer.Write(format, arg0);
        }

        public void Write(string format, object arg0, object arg1)
        {
            _writer.Write(format, arg0, arg1);
        }

        public void Write(string format, object arg0, object arg1, object arg2)
        {
            _writer.Write(format, arg0, arg1, arg2);
        }

        public void Write(string format, params object[] arg)
        {
            _writer.Write(format, arg);
        }

        public void WriteLine()
        {
            AppendTabs();
            _writer.WriteLine();
        }

        public void WriteLine(char value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(char[] buffer)
        {
            AppendTabs();
            _writer.Write(buffer);

            _writer.WriteLine();
        }

        public void WriteLine(char[] buffer, int index, int count)
        {
            AppendTabs();
            _writer.Write(buffer, index, count);

            _writer.WriteLine();
        }

        public void WriteLine(bool value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(int value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(uint value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(long value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(ulong value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(float value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(double value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(decimal value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(string value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(object value)
        {
            AppendTabs();
            _writer.Write(value);

            _writer.WriteLine();
        }

        public void WriteLine(string format, object arg0)
        {
            AppendTabs();
            _writer.Write(format, arg0);

            _writer.WriteLine();
        }

        public void WriteLine(string format, object arg0, object arg1)
        {
            AppendTabs();
            _writer.Write(format, arg0, arg1);

            _writer.WriteLine();
        }

        public void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            AppendTabs();
            _writer.Write(format, arg0, arg1, arg2);

            _writer.WriteLine();
        }

        public void WriteLine(string format, params object[] arg)
        {
            AppendTabs();
            _writer.Write(format, arg);

            _writer.WriteLine();
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reinforced.Typings.Tests.Tokenizing
{
    sealed class SimpleTokenizer
    {
        private readonly TextReader _reader;
        private readonly bool _tokenizeComments;
        private const string TokenSeparators = ".,;:{[()]}=,+-/*%@!&|";
        private bool _inComment;
        private readonly StringBuilder _buffer = new StringBuilder();

        public SimpleTokenizer(TextReader reader,bool tokenizeComments = false)
        {
            _reader = reader;
            _tokenizeComments = tokenizeComments;
        }

        public IEnumerable<string> Tokenize()
        {
            var line = _reader.ReadLine();
            while (line != null)
            {
                foreach (var token in TokenizeLine(line))
                {
                    if (!_tokenizeComments&&token.StartsWith("//")) continue;
                    yield return token;
                }
                line = _reader.ReadLine();
            }
        }

        private IEnumerable<string> TokenizeLine(string s)
        {
            if (s.Length == 1)
            {
                yield return s;
                yield break;
            }

            bool readingToken = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (Ahead(s, "//", i))
                {
                    if (_buffer.Length > 0) yield return _buffer.ToString();
                    _buffer.Clear();
                    yield return s.Substring(i);
                    yield break;
                }

                if (!_inComment)
                {
                    var commentAhead = Ahead(s, "/*", i);
                    _inComment = commentAhead;
                    if (commentAhead)
                    {
                        if (_tokenizeComments) _buffer.Append(s[i]);
                        continue;
                    }
                }

                if (_inComment)
                {
                    var commentEndAhead = Ahead(s, "*/", i);
                    if (_tokenizeComments) _buffer.Append(s[i]);
                    if (!commentEndAhead) continue;
                    _inComment = false;
                    if (_tokenizeComments)
                    {
                        _buffer.Append("*/");
                        yield return _buffer.ToString();
                        _buffer.Clear();
                    }
                    i+=2;
                    continue;
                }


                if (!char.IsWhiteSpace(s[i]))
                {
                    readingToken = true;
                    _buffer.Append(s[i]);
                }
                else
                {
                    if (readingToken)
                    {
                        yield return _buffer.ToString();
                        _buffer.Clear();
                        readingToken = false;
                    }
                }

                if (IsTokenSeparator(s[i]))
                {
                    yield return _buffer.ToString();
                    _buffer.Clear();
                    readingToken = false;
                }
            }

            if (_buffer.Length != 0)
            {
                var r = _buffer.ToString();
                _buffer.Clear();
                yield return r;
            }
        }

        private bool Ahead(string s, string lookup,int pos)
        {
            if (s.Length - pos < lookup.Length) return false;
            if (lookup.Length == 1) return s[pos] == lookup[0];

            int lp = 0;
            while (lp < lookup.Length && lp + pos < s.Length)
            {
                if (s[lp + pos] != lookup[lp]) return false;
                lp++;
            }
            return true;
        }

        private bool IsTokenSeparator(char c)
        {
            return TokenSeparators.Contains(c);
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace Reinforced.Typings.Tests.Tokenizing
{
    public static class TokenizingComparer
    {
        public static bool TokenizeCompare(this string s, string to, bool tokenizeComments = false)
        {
            using (var stringReader1 = new StringReader(s))
            {
                using (var stringReader2 = new StringReader(to))
                {
                    var t1 = new SimpleTokenizer(stringReader1);
                    var t2 = new SimpleTokenizer(stringReader2);

                    IEnumerator<string> en1 = null;
                    IEnumerator<string> en2 = null;

                    try
                    {
                        en1 = t1.Tokenize().GetEnumerator();
                        en2 = t2.Tokenize().GetEnumerator();

                        while (true)
                        {
                            var canNext1 = en1.MoveNext();
                            var canNext2 = en2.MoveNext();

                            if (canNext1 != canNext2)
                            {
                                return false;
                            }

                            if (!canNext1) break;
                            
                            var token1 = en1.Current;
                            var token2 = en2.Current;

                            if (token1 != token2) return false;
                        }

                        return true;
                    }
                    finally
                    {
                        if (en1 != null) en1.Dispose();
                        if (en2 != null) en2.Dispose();
                    }
                }
            }
        }
    }
}

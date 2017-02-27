using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Tests.Tokenizing;
using Xunit;

namespace Reinforced.Typings.Tests
{
    public class TokenizingComparerTests
    {

        [Fact]
        public void BasicTest()
        {
            string s1 = "interface ISometing { _field:number; }";
            string s2 = @"interface    ISometing    { 
_field:number;
}";
            var equal = s1.TokenizeCompare(s2);

            Assert.True(equal);
        }

        [Fact]
        public void NotEqualBasicTest()
        {
            string s1 = "interface ISometing { _field:number; }";
            string s2 = @"interface    
ISometing2    {     
    _field2: string;
}";
            var equal = s1.TokenizeCompare(s2);

            Assert.False(equal);
        }

        [Fact]
        public void InlineCommentTest()
        {
            string s1 = "interface ISometing /* a = 1 + 1 */ { _field:number; }";
            string s2 = @"interface    
ISometing    {     
    _field: number; // hello
}";
            var equal = s1.TokenizeCompare(s2);

            Assert.True(equal);
        }

        [Fact]
        public void TokenizingTest()
        {
            var source = new[] { "export", "interface", "ISomething", "{", "private", "field:", "number;", "}" };
            StringBuilder sb = new StringBuilder();
            Random r = new Random();
            foreach (var s in source)
            {
                if (r.Next(10) > 5)
                {
                    sb.Append("\t");
                }
                sb.Append(s);
                var num = r.Next(1, 10);
                sb.Append(new string(' ', num));
                if (r.Next() > 5) sb.Append('\n');
            }

            string[] result = null;
            using (var ss = new StringReader(sb.ToString()))
            {
                SimpleTokenizer st = new SimpleTokenizer(ss);
                result = st.Tokenize().ToArray();
            }

            Assert.Equal(result.Length, source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                Assert.Equal(source[i], result[i]);
            }
        }
    }
}

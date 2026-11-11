using Reinforced.Typings.Ast.TypeNames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Reinforced.Typings.Tests.Dev.Ast.TypeNames
{
    public class RtSimpleTypeNameTest
    {
        [Fact]
        public void ToString_Generic_HasWhiteSpace()
        {
            var typeName = new RtSimpleTypeName("Record", new RtSimpleTypeName("string"), new RtSimpleTypeName("boolean"));
            Assert.Equal("Record<string, boolean>", typeName.ToString());
        }
    }
}

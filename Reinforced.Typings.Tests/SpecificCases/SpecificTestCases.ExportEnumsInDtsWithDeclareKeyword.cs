using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void ExportEnumsInDtsWithDeclareKeyword()
        {
            const string result = @"
declare module Reinforced.Typings.Tests.SpecificCases {
	export enum TestEnum1 { 
		A = 0, 
		B = 1, 
		C = 2
	}
	export enum TestEnum2 { 
		C = 0, 
		D = 1, 
		E = 2
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().ExportPureTypings());
                s.ExportAsEnums(new[] { typeof(TestEnum1), typeof(TestEnum2) });
            }, result);
        }
    }
}
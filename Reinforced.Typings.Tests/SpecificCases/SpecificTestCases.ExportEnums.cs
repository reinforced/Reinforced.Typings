using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public enum TestEnum1
    {
        A, B, C
    }

    public enum TestEnum2
    {
        /// <summary>
        /// C Value
        /// </summary>
        C,

        /// <summary>
        /// D Value
        /// </summary>
        D,
        /// <summary>
        /// E Value
        /// </summary>
        E
    }
    public partial class SpecificTestCases
    {
        [Fact]
        public void ExportEnums()
        {
            const string result = @"
module Reinforced.Typings.Tests.SpecificCases {
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
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsEnums(new[] { typeof(TestEnum1), typeof(TestEnum2) });
            }, result);
        }
    }
}
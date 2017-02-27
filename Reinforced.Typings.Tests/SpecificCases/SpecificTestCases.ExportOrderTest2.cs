using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void ExportOrderTest2()
        {
            const string result2 = @"
module Reinforced.Typings.Tests.SpecificCases {
    export interface ITestInterface
	{
		String: string;
		Int: number;
	}  
    export interface ITestClass
	{
		String: string;
		Int: number;
	}	  
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
                s.ExportAsInterface<TestClass>().WithPublicProperties();
            }, result2);
        }
    }
}
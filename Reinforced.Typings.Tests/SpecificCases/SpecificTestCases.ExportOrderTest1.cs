using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.SpecificCases
{
    public partial class SpecificTestCases
    {
        [Fact]
        public void ExportOrderTest1()
        {
            const string result1 = @"
module Reinforced.Typings.Tests.SpecificCases {
    export interface ITestClass
	{
		String: string;
		Int: number;
	}	
    export interface ITestInterface
	{
		String: string;
		Int: number;
	}    
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
                s.ExportAsInterface<TestClass>().WithPublicProperties().Order(-1);
            }, result1);
        }
    }
}
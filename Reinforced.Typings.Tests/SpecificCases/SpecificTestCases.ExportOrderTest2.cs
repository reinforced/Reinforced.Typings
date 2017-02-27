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
		Int: number;
		String: string;
	}
	export interface ITestClass
	{
		String: string;
		Int: number;
	}
	export class TestClass2
	{
		public Int: number;
		public String: string;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties().WithProperty(c=>c.String).Order(1);
                s.ExportAsInterface<TestClass>().WithPublicProperties();
                s.ExportAsClass<TestClass2>().WithPublicProperties().WithProperty(c => c.Int).Order(-1);
            }, result2);
        }
    }
}
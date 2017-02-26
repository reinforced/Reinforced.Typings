using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void ExportOrderTest2()
        {
            const string result2 = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
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
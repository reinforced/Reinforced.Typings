using Reinforced.Typings.Fluent;
using Reinforced.Typings.Tests.Core;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{

    public interface ITestInterface
    {
        string String { get; }
        int Int { get; }
    }

    public class TestClass
    {
        public string String { get; set; }

        public int Int { get; set; }
    }

    public class InterfaceExporterTests : RtExporterTestBase
    {
        [Fact]
        public void InterfaceAsInterface()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export interface ITestInterface
	{
		String: string;
		Int: number;
	}
}
";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment());
                s.ExportAsInterface<ITestInterface>().WithPublicProperties();
            }, result);
        }
    }
}

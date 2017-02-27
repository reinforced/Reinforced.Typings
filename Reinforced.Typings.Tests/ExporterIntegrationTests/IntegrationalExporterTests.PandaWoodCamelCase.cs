using Reinforced.Typings.Fluent;
using Xunit;

namespace Reinforced.Typings.Tests.ExporterIntegrationTests
{
    public partial class IntegrationalExporterTests
    {
        [Fact]
        public void PandaWoodCamelCase()
        {
            const string result = @"
module Reinforced.Typings.Tests.ExporterIntegrationTests {
	export interface IPandaWoodCamelCaseTest
	{
		id: number;
		eta() : void;
		isoDate(date: any) : void;
	}
}";
            AssertConfiguration(s =>
            {
                s.Global(a => a.DontWriteWarningComment().CamelCaseForMethods().CamelCaseForProperties());
                s.ExportAsInterface<PandaWoodCamelCaseTest>().WithPublicProperties().WithPublicMethods();
            }, result);
        }
    }
}